using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ReactNative.Framework
{
    public class ModuleMethod
    {
        public string Name { get; set; }

        public string JavaScriptName { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public void Invoke(object instance, object[] parameters)
        {
            var methodParams = MethodInfo.GetParameters();

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];

                // convert JArray to generic list of method param type
                if (param is JArray)
                {
                    var arr = (JArray)param;
                    var methodParam = methodParams[i];
                    var elementType = methodParam.ParameterType.GetGenericArguments()[0];

                    var genericListType = typeof (List<>).MakeGenericType(elementType);
                    var newArray = (IList)Activator.CreateInstance(genericListType);
                    var values = arr.Values().ToArray();
                    foreach (var token in values)
                    {
                        newArray.Add(token.ToObject<object>());
                    }

                    parameters[i] = newArray;
                }
            }

            MethodInfo.Invoke(instance, parameters);
        }
    }
}