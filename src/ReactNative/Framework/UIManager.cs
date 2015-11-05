using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;

namespace ReactNative.Framework
{
    public interface IReactComponent
    {
        long? ReactTag { get; set; }
    }

    public interface IReactContainerComponent : IReactComponent
    {
        IObservableCollection<IReactComponent> Views { get; }

        void AddView(IReactComponent view);

        void AddViewAtIndex(IReactComponent view, int index);

        void RemoveView(IReactComponent child);
    }

    public interface IUIManager
    {
        void RegisterRootView(IReactComponent view);
        IReactComponent ViewForReactTag(long tag);
        void Reset();
    }

    [ReactModule("UIManager", Type=typeof(IUIManager))]
    public class UIManager : IUIManager, IBridgeModule
    {
        private readonly List<IReactComponent> _rootViews = new List<IReactComponent>();
        private readonly IDictionary<long, IReactComponent> _viewRegistry = new Dictionary<long, IReactComponent>();
        private readonly List<IViewManager> _viewManagers = new List<IViewManager>();
        private readonly List<IReactComponentData> _componentData = new List<IReactComponentData>();

        private long? _rootViewStartTag = null;
        private IReactBridge _bridge;

        public void RegisterRootView(IReactComponent view)
        {
            var rootTag = _rootViewStartTag ?? 1;
            view.ReactTag = rootTag;

            _rootViewStartTag = view.ReactTag + 10;

            _viewRegistry[rootTag] = view;

            _rootViews.Add(view);
        }

        [ReactMethod("createView")]
        public void CreateView(long reactTag, string viewName, long rootTag, IDictionary<string, object> props)
        {
            Debug.WriteLine("UIManager::CreateView {0} {1}, {2}", reactTag, viewName, JsonConvert.SerializeObject(props));

            var componentData = _componentData.Single(c => c.Name == viewName);
            var view = componentData.CreateViewWithTag(reactTag);

            componentData.SetPropsForView(props, view);

            _viewRegistry[reactTag] = view;
        }

        [ReactMethod("updateView")]
        public void UpdateView(long reactTag, string viewName, IDictionary<string, object> props)
        {
            Debug.WriteLine("UIManager::UpdateView {0} {1}, {2}", reactTag, viewName, JsonConvert.SerializeObject(props));
            var componentData = _componentData.Single(c => c.Name == viewName);
            var view = _viewRegistry[reactTag];
            componentData.SetPropsForView(props, view);
        }

        [ReactMethod("manageChildren")]
        public void ManageChildren(
            long containerReactTag,
            IList<long> moveFromIndicies,
            IList<long> moveToIndicies,
            IList<long> addChildReactTags,
            IList<long> addAtIndicies,
            IList<long> removeAtIndices)
        {
            _manageChildren(
                containerReactTag,
                moveFromIndicies ?? new List<long>(),
                moveToIndicies ?? new List<long>(),
                addChildReactTags ?? new List<long>(),
                addAtIndicies ?? new List<long>(),
                removeAtIndices ?? new List<long>(),
                _viewRegistry);
        }

        public IDictionary<string, object> ConstantsToExport()
        {
            var constants = new Dictionary<string, object>
            {
                { "customBubblingEventTypes", BubblingEventsConfig() },
                { "customDirectEventTypes", DirectEventsConfig() }
            };

            return constants;
        }

        private IDictionary<string, object> BubblingEventsConfig()
        {
            var config = new Dictionary<string, object>();

            _componentData.Apply(module =>
            {
                var manager = module.Manager;
                var events = manager.CustomBubblingEventTypes().ToList();

                events.Apply(eventName =>
                {
                    var topName = eventName.NormalizeInputEventName();
                    if (!config.ContainsKey(topName))
                    {
                        var bubbleName = "on" + topName.Substring(3);

                        config[topName] = new Dictionary<string, object>
                        {
                            { "phasedRegistrationNames", new Dictionary<string, string>
                                {
                                    { "bubbled", bubbleName},
                                    { "captured", bubbleName + "Capture"}
                                }
                            }
                        };
                    }
                });
            });

            return config;
        }

        private IDictionary<string, object> DirectEventsConfig()
        {
            var config = new Dictionary<string, object>();

            _componentData.Apply(module =>
            {
                var manager = module.Manager;
                var events = manager.CustomBubblingEventTypes().ToList();

                events.Apply(eventName =>
                {
                    var topName = eventName.NormalizeInputEventName();
                    if (!config.ContainsKey(topName))
                    {
                        var registrationName = "on" + topName.Substring(3);
                        config[topName] = new Dictionary<string, object>
                        {
                            { "registrationName", registrationName }
                        };
                    }
                });
            });
            return config;
        }

        public void _manageChildren(
            long containerReactTag,
            IList<long> moveFromIndicies,
            IList<long> moveToIndicies,
            IList<long> addChildReactTags,
            IList<long> addAtIndicies,
            IList<long> removeAtIndices,
            IDictionary<long, IReactComponent> registry)
        {
            var container = registry[containerReactTag] as IReactContainerComponent;

            Debug.Assert(container != null, "Target component should be a container");
            Debug.Assert(moveFromIndicies.Count == moveToIndicies.Count,
                "moveFromIndices had size {0}, moveToIndicies had size {1}".ToFormat(moveFromIndicies, moveToIndicies));
            Debug.Assert(addChildReactTags.Count == addAtIndicies.Count,
                "there should be at least one React child to add");

            var permanentlyRemovedChildren = _childrenToRemoveFromContainer(container, removeAtIndices);
            var temporarilyRemovedChildren = _childrenToRemoveFromContainer(container, moveFromIndicies);

            _removeChildrenFromContainer(container, permanentlyRemovedChildren);
            _removeChildrenFromContainer(container, temporarilyRemovedChildren);

            _purgeChildrenFromRegistry(registry, permanentlyRemovedChildren);

            // Figure out what to insert - merge temporary inserts and adds
            var destinationsToChildrenToAdd = new Dictionary<long, IReactComponent>();
            for (var i = 0; i < temporarilyRemovedChildren.Count; i++)
            {
                destinationsToChildrenToAdd[moveToIndicies[i]] = temporarilyRemovedChildren[i];
            }

            for (var i = 0; i < addAtIndicies.Count; i++)
            {
                var view = registry[addChildReactTags[i]];
                if (view != null)
                {
                    destinationsToChildrenToAdd[addAtIndicies[i]] = view;
                }
            }

            var sortedIndices = destinationsToChildrenToAdd.OrderBy(x => x.Key);
            foreach (var pair in sortedIndices)
            {
                container.AddViewAtIndex(pair.Value, (int)pair.Key);
            }
        }

        /// <summary>
        /// Disassociates children from container. Doesn't remove from registries.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="atIndices"></param>
        /// <returns>List of removed items.</returns>
        private IList<IReactComponent> _childrenToRemoveFromContainer(IReactContainerComponent container, IList<long> atIndices)
        {
            if (atIndices.Count == 0 || !container.Views.Any())
            {
                return new List<IReactComponent>();
            }

            var removedChildren = new List<IReactComponent>();

            atIndices.Apply(index =>
            {
                if (index < container.Views.Count)
                {
                    removedChildren.Add(container.Views[(int)index]);
                }
            });

            if (removedChildren.Count != atIndices.Count)
            {
                Debug.WriteLine("Wrong number of children removed");
            }

            return removedChildren;
        }

        private void _removeChildrenFromContainer(IReactContainerComponent container, IEnumerable<IReactComponent> children)
        {
            if (children == null)
            {
                return;
            }

            children.Apply(container.RemoveView);
        }

        private void _purgeChildrenFromRegistry(IDictionary<long, IReactComponent> registry, IEnumerable<IReactComponent> children)
        {
            if (children == null)
            {
                return;
            }

            children.Apply(child =>
            {
                Debug.Assert(!child.IsReactRootView(), "Root views should not be unregistered");

                registry[child.ReactTag.Value] = null;
            });
        }

        public IReactComponent ViewForReactTag(long tag)
        {
            return _viewRegistry[tag];
        }

        public void Initialize(IReactBridge bridge)
        {
            _bridge = bridge;

            _viewManagers.AddRange(
                bridge.Modules
                    .Where(m=>m.Instance is IViewManager)
                    .Select(m=>m.Instance as IViewManager)
            );

            _viewManagers.Apply(m =>
            {
                var componentData = new ReactComponentData(m);
                _componentData.Add(componentData);
            });
        }

        public void Reset()
        {
            _rootViews.Apply(view =>
            {
                var container = view as IReactContainerComponent;
                container?.Views.Clear();
            });

            _componentData.Clear();
            _viewManagers.Clear();

            var viewsToRemove = _viewRegistry.Where(p => !p.Value.IsReactRootView()).Select(v => v.Key).ToList();
            viewsToRemove.Apply(v => _viewRegistry.Remove(v));
        }
    }
}
