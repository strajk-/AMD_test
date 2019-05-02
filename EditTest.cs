using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Reflection;
using System.Windows;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Web;
using System.Xaml;
using System.Globalization;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace AMD_test
{
    public class XReflection
    {
        static List<Type> tl = new List<Type> ();

        static public void Init2()
        {
            XClass.InitXClass (tl);
        }
        static public void Init1()
        {
            XClass.TClasses = new List<XClass> ();
            tl = new List<Type> ();

            string myAssembly = Assembly.GetExecutingAssembly ().GetName ().Name;

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies ()) {
                if (
                    a.FullName.Contains (myAssembly) 
                    || a.FullName.Contains ("Presentation") 
                    || a.FullName.StartsWith ("WindowsBase")
                    ) {
                    
                    foreach (Module m in a.GetModules ()) {
                        foreach (Type t in m.GetTypes ()) {
                            tl.Add (t);
                        }
                    }
                }
            }
            XClass.BuildXClass ("DependencyObject", tl);
            return;
        }
    }

    public class XClass
    {
        public static List<XClass> TClasses = new List<XClass> ();

        public static XClass BuildXClass (string name, List<Type> tl)
        {

            XClass x;
            foreach (Type t in tl) {
                if (t.Name == name) {
                    tl.Remove (t);
                    x = new XClass (null, t, tl);
                    return (x);
                }
            }
            return (null);
        }
        public static void InitXClass(List<Type> tl)
        {
            List<XClass> l = new List<XClass> ();
            foreach (XClass x in TClasses) {
                if (x.type.IsClass) l.Add (x);
            }
            foreach (XClass x in l) {
                x.InitXClass2 (tl);
            }
        }

        public static XClass Find(string name)
        {
            foreach (XClass x in TClasses) {
                if (x.Name == name) {
                    return (x);
                }
            }
            return (null);
        }

        public static XClass FindFullName(string name)
        {
            foreach (XClass x in TClasses) {
                if (x.type.FullName == name) {
                    return (x);
                }
            }
            return (null);
        }
        public Type type;

        internal List<XPropVal> lProps = new List<XPropVal> ();

        private static readonly Attribute[] PropertyFilter = new Attribute[] { new PropertyFilterAttribute (PropertyFilterOptions.SetValues | PropertyFilterOptions.UnsetValues | PropertyFilterOptions.Valid) };

        public XClass(XClass dad, Type _t, List<Type> tl)
        {

            TClasses.Add (this); //_t);
            type = _t;

            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties (type, PropertyFilter);

            foreach (PropertyInfo pi in type.GetProperties ()) {
                if (pi.DeclaringType != type) {
                    continue;
                }
                new XPropVal (this, pdc.Find (pi.Name, true), pi);
            }
 
            if (tl != null) {
                List<Type> l = new List<Type> ();

                foreach (Type t in tl) {
                    if (t.IsClass && t.BaseType != null) {
                        try {
                            string n = t.BaseType.FullName;
                            if (n != null) {
                                if (type.FullName == n) {
                                    l.Add (t);
                                }
                            }
                        }
                        catch { }
                    }
                }
                foreach (Type t in l) {
                    tl.Remove (t);
                }
                foreach (Type t in l) {
                    try {
                        XClass xc = new XClass (this, t, tl);
                    }
                    catch { }
                }
            }
        }

        void InitXClass2(List<Type> tl)
        {
            foreach (XPropVal pv in lProps) {
                if (pv != null) pv.InitXClass (tl);
            }
        }

        public string Name
        {
            get { return (type.Name); }
        }
    }


    public class XPropVal
    {
        public PropertyInfo pi;

        public XPropVal(XClass dad, PropertyDescriptor _pd, PropertyInfo _pi)
        {
            dad.lProps.Add (this);
            pi = _pi;
        }

        XClass InitXClass(List<Type> tl, Type t)
        {
            XClass x;

            if (t.BaseType != null) {
                x = InitXClass (tl, t.BaseType);
                if (x != null) return (x);
            }
            x = XClass.BuildXClass (t.Name, tl);
            return (x);
        }

        internal void InitXClass(List<Type> tl)
        {
            Type t = null;
            t = pi.PropertyType;           
            if (t != null) {
                XClass x = XClass.FindFullName (t.FullName);
                if (x == null) {
                    InitXClass (tl, t);
                    x = XClass.FindFullName (t.FullName);

                    if (x == null) {
                        x = XClass.BuildXClass (t.Name, tl);
                    }
                }
            }
        }
    }


}