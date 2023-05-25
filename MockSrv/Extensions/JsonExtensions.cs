using Newtonsoft.Json;
using System.ComponentModel;
using System.Dynamic;

namespace MockSrv.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Clean and serialize/deserialize
        /// </summary>
        /// <param name="dirtyObject"></param>
        /// <returns></returns>
        public static dynamic? CleanDynamic(dynamic dirtyObject)
        {
            var partialCleaned = CleanObject(dirtyObject);

            var documentContent = JsonConvert.SerializeObject(partialCleaned);

            return JsonConvert.DeserializeObject<dynamic>(documentContent);
        }

        /// <summary>
        /// Traitement recursif d'un objet
        /// </summary>
        /// <param name="dirtyObject"></param>
        /// <returns></returns>
        private static dynamic? CleanObject(dynamic dirtyObject)
        {
            if (dirtyObject is Newtonsoft.Json.Linq.JObject)
            {
                var lstPropertiesNames = new List<string>();

                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(dirtyObject))
                    lstPropertiesNames.Add(prop.Name);

                lstPropertiesNames.Sort();

                dynamic cleanObject = new ExpandoObject();

                foreach (var prop in lstPropertiesNames)
                {
                    if (dirtyObject?[prop] is Newtonsoft.Json.Linq.JObject)
                    {
                        AddProperty(cleanObject, prop, CleanDynamic(dirtyObject?[prop]));
                    }
                    else if (dirtyObject?[prop] is Newtonsoft.Json.Linq.JArray)
                    {
                        if (((dirtyObject?[prop]) as Newtonsoft.Json.Linq.JContainer).Count > 0)
                        {
                            if (((dirtyObject?[prop]) as Newtonsoft.Json.Linq.JContainer).First.Type.ToString() == "Object")
                            {
                                // Liste d'object complexe
                                AddProperty(cleanObject, prop, CleanListeComplexe(dirtyObject?[prop]));
                            }
                            else
                            {
                                // Liste simple
                                AddProperty(cleanObject, prop, CleanListeSimple(dirtyObject?[prop]));
                            }
                        }
                        else
                        {
                            AddProperty(cleanObject, prop, dirtyObject?[prop]);
                        }
                    }
                    else
                    {
                        AddProperty(cleanObject, prop, dirtyObject?[prop].Value);
                    }
                }

                return cleanObject;
            }
            else if (dirtyObject is Newtonsoft.Json.Linq.JArray)
            {
                if (((dirtyObject) as Newtonsoft.Json.Linq.JContainer).First.Type.ToString() == "Object")
                {
                    // Liste d'object complexe
                    return CleanListeComplexe(dirtyObject);
                }
                else
                {
                    // Liste simple
                    return CleanListeSimple(dirtyObject);
                }
            }
            else
            {
                // Supprimer le contenu : Impossible qu'on se trouve dans cette situation, car on traite un Dto
                return null;
            }
        }

        /// <summary>
        /// Netoyer une liste d'objets
        /// </summary>
        /// <param name="dirtyObject"></param>
        /// <returns></returns>
        private static dynamic CleanListeComplexe(dynamic dirtyObject)
        {
            // Liste complexe
            dynamic lstComplexes = new List<dynamic>();

            for (int index = 0; index < (dirtyObject as Newtonsoft.Json.Linq.JArray).Count; index++)
            {
                var item = (dirtyObject as Newtonsoft.Json.Linq.JArray)[index];
                item = CleanDynamic(item);
                lstComplexes.Add(item);
            }

            return lstComplexes;
        }

        /// <summary>
        /// Trier une liste simple
        /// </summary>
        /// <param name="dirtyObject"></param>
        /// <returns></returns>
        private static dynamic CleanListeSimple(dynamic dirtyObject)
        {
            var lstSimple = dirtyObject.ToObject<List<dynamic>>();
            lstSimple.Sort();

            return lstSimple;
        }

        /// <summary>
        /// Injecter ou update une propriete dans un objet
        /// </summary>
        /// <param name="expando"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;

            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
