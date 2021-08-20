using System.Collections.Generic;
using System.Linq;

namespace SpreetailMultiValueDictionary.Services
{
    public class MultiValueDictionaryService : IMultiValueDictionaryService
    {
        private static IDictionary<string, IList<string>> MultiValueDictionary { get; set; } = new Dictionary<string, IList<string>>();

        public IEnumerable<string> GetKeys()
        {
            return MultiValueDictionary.Keys.Any() ? MultiValueDictionary.Keys : null;
        }

        public IEnumerable<string> GetKeyMembers(string key)
        {
            return MultiValueDictionary.TryGetValue(key, out var values) ? values : null;
        }

        public bool AddKeyMember(string key, string member)
        {
            if (MultiValueDictionary.ContainsKey(key))
            {
                if (MultiValueDictionary[key].Contains(member))
                {
                    return false;
                }
                MultiValueDictionary[key].Add(member);
            }
            else
            {
                MultiValueDictionary.Add(key, new List<string> { member });
            }

            return true;
        }

        public bool? RemoveKeyMember(string key, string member)
        {
            if (!MultiValueDictionary.ContainsKey(key))
                return null;

            if (!MultiValueDictionary[key].Contains(member)) 
                return false;

            MultiValueDictionary[key].Remove(member);
            if (!MultiValueDictionary[key].Any())
                MultiValueDictionary.Remove(key);
            return true;
        }

        public bool RemoveAllKeyMembers(string key)
        {
            if (!MultiValueDictionary.ContainsKey(key))
                return false;

            MultiValueDictionary.Remove(key);
            return true;
        }

        public void RemoveAllKeys()
        {
            foreach (var key in MultiValueDictionary.Keys)
            {
                MultiValueDictionary.Remove(key);
            }
        }

        public bool KeyExists(string key)
        {
            return MultiValueDictionary.ContainsKey(key);
        }

        public bool? KeyMemberExists(string key, string member)
        {
            if (MultiValueDictionary.TryGetValue(key, out var values))
            {
                return values.Contains(member);
            }

            return null;
        }

        public IEnumerable<string> GetAllMembers()
        {
            return MultiValueDictionary.SelectMany(x => x.Value);
        }

        public IEnumerable<string> GetAllItems()
        {
            return MultiValueDictionary.Select(x => $"{x.Key}: {x.Value}");
        }
    }
}
