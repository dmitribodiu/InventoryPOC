using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Events.Account
{
    public class Account
    {
        protected List<AccountComponent> Children = new List<AccountComponent>();

        public void Add(AccountComponent component)
        {
            Children.Add(component);
        }

        public void Remove(AccountComponent component)
        {
            Children.Remove(component);
        }

        public Guid GetId()
        {
            var lastAccount = Children.Last();
            return lastAccount.GetId();
        }

        public static Account Parse(string accountAsString)
        {
            var account = new Account();

            var accountComponents = GetAccountComponent(accountAsString);

            foreach (var accountComponent in accountComponents)
            {
                account.Add(accountComponent);
            }

            return account;
        }

        public override string ToString()
        {
            return String.Join(":", Children.Select(x=>x.ToString()));
        }

        private static List<AccountComponent> GetAccountComponent(string accountAsString)
        {
            var accountComponents = accountAsString.Split(":");

            List<AccountComponent> objects = new List<AccountComponent>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(AccountComponent)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(AccountComponent))))
            {
                var component = (AccountComponent)Activator.CreateInstance(type);
                foreach (var accountComponent in accountComponents)
                {
                    if (component.TryParse(accountComponent, out AccountComponent returnedComponent))
                    {
                        objects.Add(returnedComponent);
                    }
                }
            }
            return objects;
        }

        public T GetComponent<T>() where T : class
        {
            var accountComponent = Children.Single(x => x is T);
            return accountComponent as T;
        }

        public T TryGetComponent<T>() where T : class
        {
            var accountComponent = Children.SingleOrDefault(x => x is T);
            return accountComponent as T;
        }

        public bool ContainsComponent<T>()
        {
            var component = Children.SingleOrDefault(x => x is T);
            return component != null;
        }
    }
}
