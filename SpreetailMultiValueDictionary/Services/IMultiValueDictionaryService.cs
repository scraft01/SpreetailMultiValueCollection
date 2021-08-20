using System.Collections.Generic;

namespace SpreetailMultiValueDictionary.Services
{
    public interface IMultiValueDictionaryService
    {
        IEnumerable<string> GetKeys();
        IEnumerable<string> GetKeyMembers(string key);
        bool AddKeyMember(string key, string member);
        bool? RemoveKeyMember(string key, string member);
        bool RemoveAllKeyMembers(string key);
        void RemoveAllKeys();
        bool KeyExists(string key);
        bool? KeyMemberExists(string key, string member);
        IEnumerable<string> GetAllMembers();
        IEnumerable<string> GetAllItems();
    }
}
