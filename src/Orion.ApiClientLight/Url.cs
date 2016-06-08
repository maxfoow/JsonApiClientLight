using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Orion.ApiClientLight
{
    public class Url
    {
        private readonly string _url;
        private readonly object _parameters;

        public Url(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            _url = url;
        }

        public Url(string url, object parameters)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            _url = url;
            _parameters = parameters;
        }

        public override string ToString()
        {
            if (_parameters == null)
            {
                return _url;
            }
            if (_parameters is string)
            {
                return _url + _parameters;
            }
            IDictionary<string, string> parameters;
            if (_parameters is ExpandoObject)
            {
                parameters = GetFromExpandoObject((ExpandoObject) _parameters);
            }
            else
            {
                parameters = GetFromObject(_parameters);
            }

            return _url + ConvertParametersDictionnaryToString(parameters);
        }


        private string ConvertParametersDictionnaryToString(IDictionary<string, string> parameters)
        {
            if (!parameters.Any())
            {
                return string.Empty;
            }
            var result = new StringBuilder("?");
            var count = 0;
            foreach (var parameter in parameters)
            {
                if (count > 0)
                    result.Append("&");
                result.Append($"{parameter.Key}={parameter.Value}");
                ++count;
            }
            return result.ToString();
        }

        private IDictionary<string, string> GetFromExpandoObject(ExpandoObject parameters)
        {
            var results = new Dictionary<string, string>();
            var dictionnary = (IDictionary<string, object>) parameters;
            foreach (var keyPair in dictionnary)
            {
                if (keyPair.Value.GetType().GetTypeInfo().IsPrimitive)
                    results.Add(keyPair.Key, keyPair.Value.ToString());
                else if (keyPair.Value is string)
                    results.Add(keyPair.Key, (string) keyPair.Value);
                else
                    results.Add(keyPair.Key, JsonConvert.SerializeObject(keyPair.Value));
            }
            return results;
        }
        private IDictionary<string, string> GetFromObject(object parameters)
        {
            var results = new Dictionary<string, string>();
            var type = parameters.GetType();
            foreach (var propertyInfo in type.GetTypeInfo().DeclaredProperties)
            {
                var value = propertyInfo.GetValue(parameters);
                if (value.GetType().GetTypeInfo().IsPrimitive)
                    results.Add(propertyInfo.Name, value.ToString());
                else if (value is string)
                    results.Add(propertyInfo.Name, (string) value);
                else
                    results.Add(propertyInfo.Name, JsonConvert.SerializeObject(value));
            }
            return results;
        }
    }
}