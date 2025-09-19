// From Package VHierarchy 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VV.Extern.Utility
{
    public static class VUtils
    {

        #region Reflection


        public static object GetFieldValue(this object o, string fieldName)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(fieldName) is FieldInfo fieldInfo)
                return fieldInfo.GetValue(target);


            throw new System.Exception($"Field '{fieldName}' not found in type '{type.Name}' and its parent types");

        }

        public static object GetPropertyValue(this object o, string propertyName)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetPropertyInfo(propertyName) is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(target);


            throw new System.Exception(
                $"Property '{propertyName}' not found in type '{type.Name}' and its parent types");

        }

        public static object GetMemberValue(this object o, string memberName)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(memberName) is FieldInfo fieldInfo)
                return fieldInfo.GetValue(target);

            if (type.GetPropertyInfo(memberName) is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(target);


            throw new System.Exception($"Member '{memberName}' not found in type '{type.Name}' and its parent types");

        }

        public static void SetFieldValue(this object o, string fieldName, object value)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(fieldName) is FieldInfo fieldInfo)
                fieldInfo.SetValue(target, value);


            else
                throw new System.Exception($"Field '{fieldName}' not found in type '{type.Name}' and its parent types");

        }

        public static void SetPropertyValue(this object o, string propertyName, object value)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetPropertyInfo(propertyName) is PropertyInfo propertyInfo)
                propertyInfo.SetValue(target, value);


            else
                throw new System.Exception(
                    $"Property '{propertyName}' not found in type '{type.Name}' and its parent types");

        }

        public static void SetMemberValue(this object o, string memberName, object value)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(memberName) is FieldInfo fieldInfo)
                fieldInfo.SetValue(target, value);

            else if (type.GetPropertyInfo(memberName) is PropertyInfo propertyInfo)
                propertyInfo.SetValue(target, value);


            else
                throw new System.Exception(
                    $"Member '{memberName}' not found in type '{type.Name}' and its parent types");

        }

        public static object
            InvokeMethod(this object o, string methodName,
                params object[] parameters) // todo handle null params (can't get their type)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetMethodInfo(methodName, parameters.Select(r => r.GetType()).ToArray()) is MethodInfo methodInfo)
                return methodInfo.Invoke(target, parameters);


            throw new System.Exception(
                $"Method '{methodName}' not found in type '{type.Name}', its parent types and interfaces");

        }


        public static T GetFieldValue<T>(this object o, string fieldName) => (T)o.GetFieldValue(fieldName);
        public static T GetPropertyValue<T>(this object o, string propertyName) => (T)o.GetPropertyValue(propertyName);
        public static T GetMemberValue<T>(this object o, string memberName) => (T)o.GetMemberValue(memberName);

        public static T InvokeMethod<T>(this object o, string methodName, params object[] parameters) =>
            (T)o.InvokeMethod(methodName, parameters);




        public static FieldInfo GetFieldInfo(this Type type, string fieldName)
        {
            if (fieldInfoCache.TryGetValue(type, out var fieldInfosByNames))
                if (fieldInfosByNames.TryGetValue(fieldName, out var fieldInfo))
                    return fieldInfo;


            if (!fieldInfoCache.ContainsKey(type))
                fieldInfoCache[type] = new Dictionary<string, FieldInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetField(fieldName, maxBindingFlags) is FieldInfo fieldInfo)
                    return fieldInfoCache[type][fieldName] = fieldInfo;


            return fieldInfoCache[type][fieldName] = null;

        }

        public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            if (propertyInfoCache.TryGetValue(type, out var propertyInfosByNames))
                if (propertyInfosByNames.TryGetValue(propertyName, out var propertyInfo))
                    return propertyInfo;


            if (!propertyInfoCache.ContainsKey(type))
                propertyInfoCache[type] = new Dictionary<string, PropertyInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetProperty(propertyName, maxBindingFlags) is PropertyInfo propertyInfo)
                    return propertyInfoCache[type][propertyName] = propertyInfo;


            return propertyInfoCache[type][propertyName] = null;

        }

        public static MethodInfo GetMethodInfo(this Type type, string methodName, params Type[] argumentTypes)
        {
            var methodHash = methodName.GetHashCode() ^
                             argumentTypes.Aggregate(0, (hash, r) => hash ^= r.GetHashCode());


            if (methodInfoCache.TryGetValue(type, out var methodInfosByHashes))
                if (methodInfosByHashes.TryGetValue(methodHash, out var methodInfo))
                    return methodInfo;



            if (!methodInfoCache.ContainsKey(type))
                methodInfoCache[type] = new Dictionary<int, MethodInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetMethod(methodName, maxBindingFlags, null, argumentTypes, null) is MethodInfo methodInfo)
                    return methodInfoCache[type][methodHash] = methodInfo;

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.GetMethod(methodName, maxBindingFlags, null, argumentTypes, null) is MethodInfo
                    methodInfo)
                    return methodInfoCache[type][methodHash] = methodInfo;



            return methodInfoCache[type][methodHash] = null;

        }

        static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoCache = new();
        static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyInfoCache = new();
        static Dictionary<Type, Dictionary<int, MethodInfo>> methodInfoCache = new();






        public static T GetCustomAttributeCached<T>(this MemberInfo memberInfo) where T : System.Attribute
        {
            if (!attributesCache.TryGetValue(memberInfo, out var attributes_byType))
                attributes_byType = attributesCache[memberInfo] = new();

            if (!attributes_byType.TryGetValue(typeof(T), out var attribute))
                attribute = attributes_byType[typeof(T)] = memberInfo.GetCustomAttribute<T>();

            return attribute as T;

        }

        static Dictionary<MemberInfo, Dictionary<Type, System.Attribute>> attributesCache = new();






        public static List<Type> GetSubclasses(this Type t) =>
            t.Assembly.GetTypes().Where(type => type.IsSubclassOf(t)).ToList();

        public static object GetDefaultValue(this FieldInfo f, params object[] constructorVars) =>
            f.GetValue(System.Activator.CreateInstance(((MemberInfo)f).ReflectedType, constructorVars));

        public static object GetDefaultValue(this FieldInfo f) =>
            f.GetValue(System.Activator.CreateInstance(((MemberInfo)f).ReflectedType));

        public static IEnumerable<FieldInfo> GetFieldsWithoutBase(this Type t) =>
            t.GetFields().Where(r => !t.BaseType.GetFields().Any(rr => rr.Name == r.Name));

        public static IEnumerable<PropertyInfo> GetPropertiesWithoutBase(this Type t) => t.GetProperties()
            .Where(r => !t.BaseType.GetProperties().Any(rr => rr.Name == r.Name));


        public const BindingFlags maxBindingFlags = (BindingFlags)62;








        #endregion

        #region Collections


        public static class CollectionUtils
        {
            public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(
                IEnumerable<Dictionary<TKey, TValue>> dicts)
            {
                if (dicts.Count() == 0) return null;
                if (dicts.Count() == 1) return dicts.First();

                var mergedDict = new Dictionary<TKey, TValue>(dicts.First());

                foreach (var dict in dicts.Skip(1))
                foreach (var r in dict)
                    if (!mergedDict.ContainsKey(r.Key))
                        mergedDict.Add(r.Key, r.Value);

                return mergedDict;
            }

        }


        public static T NextTo<T>(this IEnumerable<T> e, T to) =>
            e.SkipWhile(r => !r.Equals(to)).Skip(1).FirstOrDefault();

        public static T PreviousTo<T>(this IEnumerable<T> e, T to) =>
            e.Reverse().SkipWhile(r => !r.Equals(to)).Skip(1).FirstOrDefault();

        public static T NextToOrFirst<T>(this IEnumerable<T> e, T to) => e.NextTo(to) ?? e.First();
        public static T PreviousToOrLast<T>(this IEnumerable<T> e, T to) => e.PreviousTo(to) ?? e.Last();

        public static IEnumerable<T> InsertFirst<T>(this IEnumerable<T> ie, T t) => new[] { t }.Concat(ie);

        public static int IndexOfFirst<T>(this List<T> list, System.Func<T, bool> f) =>
            list.FirstOrDefault(f) is T t ? list.IndexOf(t) : -1;

        public static int IndexOfLast<T>(this List<T> list, System.Func<T, bool> f) =>
            list.LastOrDefault(f) is T t ? list.IndexOf(t) : -1;

        public static void SortBy<T, T2>(this List<T> list, System.Func<T, T2> keySelector)
            where T2 : System.IComparable => list.Sort((q, w) => keySelector(q).CompareTo(keySelector(w)));

        public static void RemoveValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            if (dictionary.FirstOrDefault(r => r.Value.Equals(value)) is var kvp)
                dictionary.Remove(kvp);
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, System.Action<T> action)
        {
            foreach (T item in sequence) action(item);
        }



        public static T AddAt<T>(this List<T> l, T r, int i)
        {
            if (i < 0) i = 0;
            if (i >= l.Count)
                l.Add(r);
            else
                l.Insert(i, r);
            return r;
        }

        public static T RemoveLast<T>(this List<T> l)
        {
            if (!l.Any()) return default;

            var r = l.Last();

            l.RemoveAt(l.Count - 1);

            return r;
        }

        public static void Add<T>(this List<T> list, params T[] items)
        {
            foreach (var r in items)
                list.Add(r);
        }






        #endregion

        #region Math


        public static class MathUtil // MathUtils name is taken by UnityEditor.MathUtils 
        {

            public static float TriangleArea(Vector2 A, Vector2 B, Vector2 C) =>
                Vector3.Cross(A - B, A - C).z.Abs() / 2;

            public static Vector2 LineIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
            {
                var a1 = B.y - A.y;
                var b1 = A.x - B.x;
                var c1 = a1 * A.x + b1 * A.y;

                var a2 = D.y - C.y;
                var b2 = C.x - D.x;
                var c2 = a2 * C.x + b2 * C.y;

                var d = a1 * b2 - a2 * b1;

                var x = (b2 * c1 - b1 * c2) / d;
                var y = (a1 * c2 - a2 * c1) / d;

                return new Vector2(x, y);

            }




            public static float Lerp(float f1, float f2, float t) => Mathf.LerpUnclamped(f1, f2, t);

            public static float Lerp(ref float f1, float f2, float t)
            {
                return f1 = Lerp(f1, f2, t);
            }

            public static Vector2 Lerp(Vector2 f1, Vector2 f2, float t) => Vector2.LerpUnclamped(f1, f2, t);

            public static Vector2 Lerp(ref Vector2 f1, Vector2 f2, float t)
            {
                return f1 = Lerp(f1, f2, t);
            }

            public static Vector3 Lerp(Vector3 f1, Vector3 f2, float t) => Vector3.LerpUnclamped(f1, f2, t);

            public static Vector3 Lerp(ref Vector3 f1, Vector3 f2, float t)
            {
                return f1 = Lerp(f1, f2, t);
            }

            public static Color Lerp(Color f1, Color f2, float t) => Color.LerpUnclamped(f1, f2, t);

            public static Color Lerp(ref Color f1, Color f2, float t)
            {
                return f1 = Lerp(f1, f2, t);
            }


            public static float Lerp(float current, float target, float speed, float deltaTime) =>
                Mathf.Lerp(current, target, GetLerpT(speed, deltaTime));

            public static float Lerp(ref float current, float target, float speed, float deltaTime)
            {
                return current = Lerp(current, target, speed, deltaTime);
            }

            public static Vector2 Lerp(Vector2 current, Vector2 target, float speed, float deltaTime) =>
                Vector2.Lerp(current, target, GetLerpT(speed, deltaTime));

            public static Vector2 Lerp(ref Vector2 current, Vector2 target, float speed, float deltaTime)
            {
                return current = Lerp(current, target, speed, deltaTime);
            }

            public static Vector3 Lerp(Vector3 current, Vector3 target, float speed, float deltaTime) =>
                Vector3.Lerp(current, target, GetLerpT(speed, deltaTime));

            public static Vector3 Lerp(ref Vector3 current, Vector3 target, float speed, float deltaTime)
            {
                return current = Lerp(current, target, speed, deltaTime);
            }

            public static float SmoothDamp(float current, float target, float speed, ref float derivative,
                float deltaTime, float maxSpeed) =>
                Mathf.SmoothDamp(current, target, ref derivative, .5f / speed, maxSpeed, deltaTime);

            public static float SmoothDamp(float current, float target, float speed, ref float derivative,
                float deltaTime)
            {
                return Mathf.SmoothDamp(current, target, ref derivative, .5f / speed, Mathf.Infinity, deltaTime);
            }

            public static float SmoothDamp(float current, float target, float speed, ref float derivative)
            {
                return SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }

            public static float SmoothDamp(ref float current, float target, float speed, ref float derivative,
                float deltaTime, float maxSpeed)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, deltaTime, maxSpeed);
            }

            public static float SmoothDamp(ref float current, float target, float speed, ref float derivative,
                float deltaTime)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, deltaTime);
            }

            public static float SmoothDamp(ref float current, float target, float speed, ref float derivative)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }

            public static Vector2 SmoothDamp(Vector2 current, Vector2 target, float speed, ref Vector2 derivative,
                float deltaTime) => Vector2.SmoothDamp(current, target, ref derivative, .5f / speed, Mathf.Infinity,
                deltaTime);

            public static Vector2 SmoothDamp(Vector2 current, Vector2 target, float speed, ref Vector2 derivative)
            {
                return SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }

            public static Vector2 SmoothDamp(ref Vector2 current, Vector2 target, float speed, ref Vector2 derivative,
                float deltaTime)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, deltaTime);
            }

            public static Vector2 SmoothDamp(ref Vector2 current, Vector2 target, float speed, ref Vector2 derivative)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }

            public static Vector3 SmoothDamp(Vector3 current, Vector3 target, float speed, ref Vector3 derivative,
                float deltaTime) => Vector3.SmoothDamp(current, target, ref derivative, .5f / speed, Mathf.Infinity,
                deltaTime);

            public static Vector3 SmoothDamp(Vector3 current, Vector3 target, float speed, ref Vector3 derivative)
            {
                return SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }

            public static Vector3 SmoothDamp(ref Vector3 current, Vector3 target, float speed, ref Vector3 derivative,
                float deltaTime)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, deltaTime);
            }

            public static Vector3 SmoothDamp(ref Vector3 current, Vector3 target, float speed, ref Vector3 derivative)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }


            public static float GetLerpT(float lerpSpeed, float deltaTime) =>
                1 - Mathf.Exp(-lerpSpeed * 2f * deltaTime);

            public static float GetLerpT(float lerpSpeed)
            {
                return GetLerpT(lerpSpeed, Time.deltaTime);
            }



        }


        public static float DistanceTo(this float f1, float f2) => Mathf.Abs(f1 - f2);
        public static float DistanceTo(this Vector2 f1, Vector2 f2) => (f1 - f2).magnitude;
        public static float DistanceTo(this Vector3 f1, Vector3 f2) => (f1 - f2).magnitude;

        public static float Sign(this float f) => f == 0 ? 0 : Mathf.Sign(f);

        public static int Abs(this int f) => Mathf.Abs(f);
        public static float Abs(this float f) => Mathf.Abs(f);

        public static int Clamp(this int f, int f0, int f1) => Mathf.Clamp(f, f0, f1);
        public static float Clamp(this float f, float f0, float f1) => Mathf.Clamp(f, f0, f1);


        public static float Clamp01(this float f) => Mathf.Clamp(f, 0, 1);
        public static Vector2 Clamp01(this Vector2 f) => new(f.x.Clamp01(), f.y.Clamp01());
        public static Vector3 Clamp01(this Vector3 f) => new(f.x.Clamp01(), f.y.Clamp01(), f.z.Clamp01());


        public static int Pow(this int f, int pow) => (int)Mathf.Pow(f, pow);
        public static float Pow(this float f, float pow) => Mathf.Pow(f, pow);

        public static float Round(this float f) => Mathf.Round(f);
        public static float Ceil(this float f) => Mathf.Ceil(f);
        public static float Floor(this float f) => Mathf.Floor(f);

        public static int RoundToInt(this float f) => Mathf.RoundToInt(f);
        public static int CeilToInt(this float f) => Mathf.CeilToInt(f);
        public static int FloorToInt(this float f) => Mathf.FloorToInt(f);

        public static int ToInt(this float f) => (int)f;
        public static float ToFloat(this int f) => (float)f;
        public static float ToFloat(this double f) => (float)f;



        public static float Sqrt(this float f) => Mathf.Sqrt(f);

        public static int Max(this int f, int ff) => Mathf.Max(f, ff);
        public static int Min(this int f, int ff) => Mathf.Min(f, ff);
        public static float Max(this float f, float ff) => Mathf.Max(f, ff);
        public static float Min(this float f, float ff) => Mathf.Min(f, ff);

        public static float ClampMin(this float f, float limitMin) => Mathf.Max(f, limitMin);
        public static float ClampMax(this float f, float limitMax) => Mathf.Min(f, limitMax);

        public static Vector3 ClampMaxMagnitude(this Vector3 v, float maxMagnitude)
        {
            if (v.sqrMagnitude <= maxMagnitude * maxMagnitude)
                return v;
            else
                return v.normalized * maxMagnitude;
        }


        public static float Loop(this float f, float boundMin, float boundMax)
        {
            while (f < boundMin) f += boundMax - boundMin;
            while (f > boundMax) f -= boundMax - boundMin;
            return f;
        }

        public static float Loop(this float f, float boundMax) => f.Loop(0, boundMax);

        public static float PingPong(this float f, float boundMin, float boundMax) =>
            boundMin + Mathf.PingPong(f - boundMin, boundMax - boundMin);

        public static float PingPong(this float f, float boundMax) => f.PingPong(0, boundMax);


        public static float Dot(this Vector3 v1, Vector3 v2) => Vector3.Dot(v1, v2);

        public static Vector3 Cross(this Vector3 v1, Vector3 v2) => Vector3.Cross(v1, v2);


        public static Vector2 ProjectOn(this Vector2 v, Vector2 on) => Vector3.Project(v, on);
        public static Vector3 ProjectOn(this Vector3 v, Vector3 on) => Vector3.Project(v, on);

        public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 normal) => Vector3.ProjectOnPlane(v, normal);


        public static float AngleTo(this Vector2 v, Vector2 to) => Vector2.Angle(v, to);

        public static Vector2 Rotate(this Vector2 v, float deg) => Quaternion.AngleAxis(deg, Vector3.forward) * v;

        public static float Smoothstep(this float f)
        {
            f = f.Clamp01();
            return f * f * (3 - 2 * f);
        }

        public static float InverseLerp(this Vector2 v, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            var av = v - a;
            return Vector2.Dot(av, ab) / Vector2.Dot(ab, ab);
        }


        public static bool IsOdd(this int i) => i % 2 == 1;
        public static bool IsEven(this int i) => i % 2 == 0;

        public static bool IsInRange(this int i, int a, int b) => i >= a && i <= b;
        public static bool IsInRange(this float i, float a, float b) => i >= a && i <= b;

        public static bool IsInRangeOf(this int i, IList list) => i.IsInRange(0, list.Count - 1);
        public static bool IsInRangeOf<T>(this int i, T[] array) => i.IsInRange(0, array.Length - 1);

        public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);



        [System.Serializable]
        public class GaussianKernel
        {
            public static float[,] GenerateArray(int size, float sharpness = .5f)
            {
                float[,] kr = new float[size, size];

                if (size == 1)
                {
                    kr[0, 0] = 1;
                    return kr;
                }


                var sigma = 1f - Mathf.Pow(sharpness, .1f) * .99999f;
                var radius = (size / 2f).FloorToInt();


                var a = -2f * radius * radius / Mathf.Log(sigma);
                var sum = 0f;

                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    var rX = size % 2 == 1 ? (x - radius) : (x - radius) + .5f;
                    var rY = size % 2 == 1 ? (y - radius) : (y - radius) + .5f;
                    var dist = Mathf.Sqrt(rX * rX + rY * rY);
                    kr[x, y] = Mathf.Exp(-dist * dist / a);
                    sum += kr[x, y];
                }

                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    kr[x, y] /= sum;

                return kr;
            }



            public GaussianKernel(bool isEvenSize = false, int radius = 7, float sharpness = .5f)
            {
                this.isEvenSize = isEvenSize;
                this.radius = radius;
                this.sharpness = sharpness;
            }

            public bool isEvenSize = false;
            public int radius = 7;
            public float sharpness = .5f;

            public int size => radius * 2 + (isEvenSize ? 0 : 1);
            public float sigma => 1 - Mathf.Pow(sharpness, .1f) * .99999f;

            public float[,] Array2d() // todo test and use GenerateArray
            {
                float[,] kr = new float[size, size];

                if (size == 1)
                {
                    kr[0, 0] = 1;
                    return kr;
                }

                var a = -2f * radius * radius / Mathf.Log(sigma);
                var sum = 0f;

                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    var rX = size % 2 == 1 ? (x - radius) : (x - radius) + .5f;
                    var rY = size % 2 == 1 ? (y - radius) : (y - radius) + .5f;
                    var dist = Mathf.Sqrt(rX * rX + rY * rY);
                    kr[x, y] = Mathf.Exp(-dist * dist / a);
                    sum += kr[x, y];
                }

                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    kr[x, y] /= sum;

                return kr;
            }

            public float[] ArrayFlat()
            {
                var gk = Array2d();
                float[] flat = new float[size * size];

                for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    flat[(i * size + j)] = gk[i, j];

                return flat;
            }

        }







        #endregion

        #region Rects


        public static Rect Resize(this Rect rect, float px)
        {
            rect.x += px;
            rect.y += px;
            rect.width -= px * 2;
            rect.height -= px * 2;
            return rect;
        }

        public static Rect SetPos(this Rect rect, Vector2 v) => rect.SetPos(v.x, v.y);

        public static Rect SetPos(this Rect rect, float x, float y)
        {
            rect.x = x;
            rect.y = y;
            return rect;
        }

        public static Rect SetX(this Rect rect, float x) => rect.SetPos(x, rect.y);
        public static Rect SetY(this Rect rect, float y) => rect.SetPos(rect.x, y);

        public static Rect SetXMax(this Rect rect, float xMax)
        {
            rect.xMax = xMax;
            return rect;
        }

        public static Rect SetYMax(this Rect rect, float yMax)
        {
            rect.yMax = yMax;
            return rect;
        }

        public static Rect SetMidPos(this Rect r, Vector2 v) => r.SetPos(v).MoveX(-r.width / 2).MoveY(-r.height / 2);
        public static Rect SetMidPos(this Rect r, float x, float y) => r.SetMidPos(new Vector2(x, y));

        public static Rect Move(this Rect rect, Vector2 v)
        {
            rect.position += v;
            return rect;
        }

        public static Rect Move(this Rect rect, float x, float y)
        {
            rect.x += x;
            rect.y += y;
            return rect;
        }

        public static Rect MoveX(this Rect rect, float px)
        {
            rect.x += px;
            return rect;
        }

        public static Rect MoveY(this Rect rect, float px)
        {
            rect.y += px;
            return rect;
        }

        public static Rect SetWidth(this Rect rect, float f)
        {
            rect.width = f;
            return rect;
        }

        public static Rect SetWidthFromMid(this Rect rect, float px)
        {
            rect.x += rect.width / 2;
            rect.width = px;
            rect.x -= rect.width / 2;
            return rect;
        }

        public static Rect SetWidthFromRight(this Rect rect, float px)
        {
            rect.x += rect.width;
            rect.width = px;
            rect.x -= rect.width;
            return rect;
        }

        public static Rect SetHeight(this Rect rect, float f)
        {
            rect.height = f;
            return rect;
        }

        public static Rect SetHeightFromMid(this Rect rect, float px)
        {
            rect.y += rect.height / 2;
            rect.height = px;
            rect.y -= rect.height / 2;
            return rect;
        }

        public static Rect SetHeightFromBottom(this Rect rect, float px)
        {
            rect.y += rect.height;
            rect.height = px;
            rect.y -= rect.height;
            return rect;
        }

        public static Rect AddWidth(this Rect rect, float f) => rect.SetWidth(rect.width + f);
        public static Rect AddWidthFromMid(this Rect rect, float f) => rect.SetWidthFromMid(rect.width + f);
        public static Rect AddWidthFromRight(this Rect rect, float f) => rect.SetWidthFromRight(rect.width + f);

        public static Rect AddHeight(this Rect rect, float f) => rect.SetHeight(rect.height + f);
        public static Rect AddHeightFromMid(this Rect rect, float f) => rect.SetHeightFromMid(rect.height + f);
        public static Rect AddHeightFromBottom(this Rect rect, float f) => rect.SetHeightFromBottom(rect.height + f);

        public static Rect SetSize(this Rect rect, Vector2 v) => rect.SetWidth(v.x).SetHeight(v.y);
        public static Rect SetSize(this Rect rect, float w, float h) => rect.SetWidth(w).SetHeight(h);

        public static Rect SetSize(this Rect rect, float f)
        {
            rect.height = rect.width = f;
            return rect;
        }

        public static Rect SetSizeFromMid(this Rect r, Vector2 v) => r.Move(r.size / 2).SetSize(v).Move(-v / 2);
        public static Rect SetSizeFromMid(this Rect r, float x, float y) => r.SetSizeFromMid(new Vector2(x, y));
        public static Rect SetSizeFromMid(this Rect r, float f) => r.SetSizeFromMid(new Vector2(f, f));

        public static Rect AlignToPixelGrid(this Rect r) => GUIUtility.AlignRectToDevice(r);





        #endregion

        #region Vectors


        public static Vector2 AddX(this Vector2 v, float f) => new(v.x + f, v.y + 0);
        public static Vector2 AddY(this Vector2 v, float f) => new(v.x + 0, v.y + f);

        public static Vector3 AddX(this Vector3 v, float f) => new(v.x + f, v.y + 0, v.z + 0);
        public static Vector3 AddY(this Vector3 v, float f) => new(v.x + 0, v.y + f, v.z + 0);
        public static Vector3 AddZ(this Vector3 v, float f) => new(v.x + 0, v.y + 0, v.z + f);

        public static Vector3 SetX(this Vector3 v, float f) => new(f, v.y, v.z);
        public static Vector3 SetY(this Vector3 v, float f) => new(v.x, f, v.z);
        public static Vector3 SetZ(this Vector3 v, float f) => new(v.x, v.y, f);

        public static Vector2 xx(this Vector3 v)
        {
            return new Vector2(v.x, v.x);
        }

        public static Vector2 xy(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 xz(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector2 yx(this Vector3 v)
        {
            return new Vector2(v.y, v.x);
        }

        public static Vector2 yy(this Vector3 v)
        {
            return new Vector2(v.y, v.y);
        }

        public static Vector2 yz(this Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }

        public static Vector2 zx(this Vector3 v)
        {
            return new Vector2(v.z, v.x);
        }

        public static Vector2 zy(this Vector3 v)
        {
            return new Vector2(v.z, v.y);
        }

        public static Vector2 zz(this Vector3 v)
        {
            return new Vector2(v.z, v.z);
        }





        #endregion

        #region Colors


        public class ColorUtils
        {
            public static Color HSLToRGB(float h, float s, float l)
            {
                float hue2Rgb(float v1, float v2, float vH)
                {
                    if (vH < 0f)
                        vH += 1f;

                    if (vH > 1f)
                        vH -= 1f;

                    if (6f * vH < 1f)
                        return v1 + (v2 - v1) * 6f * vH;

                    if (2f * vH < 1f)
                        return v2;

                    if (3f * vH < 2f)
                        return v1 + (v2 - v1) * (2f / 3f - vH) * 6f;

                    return v1;
                }

                if (s.Approx(0)) return new Color(l, l, l);

                float k1;

                if (l < .5f)
                    k1 = l * (1f + s);
                else
                    k1 = l + s - s * l;


                var k2 = 2f * l - k1;

                float r, g, b;
                r = hue2Rgb(k2, k1, h + 1f / 3);
                g = hue2Rgb(k2, k1, h);
                b = hue2Rgb(k2, k1, h - 1f / 3);

                return new Color(r, g, b);
            }

            public static Color LCHtoRGB(float l, float c, float h)
            {
                l *= 100;
                c *= 100;
                h *= 360;

                double xw = 0.948110;
                double yw = 1.00000;
                double zw = 1.07304;

                float a = c * Mathf.Cos(Mathf.Deg2Rad * h);
                float b = c * Mathf.Sin(Mathf.Deg2Rad * h);

                float fy = (l + 16) / 116;
                float fx = fy + (a / 500);
                float fz = fy - (b / 200);

                float x = (float)System.Math.Round(
                    xw * ((System.Math.Pow(fx, 3) > 0.008856) ? System.Math.Pow(fx, 3) : ((fx - 16 / 116) / 7.787)), 5);
                float y = (float)System.Math.Round(
                    yw * ((System.Math.Pow(fy, 3) > 0.008856) ? System.Math.Pow(fy, 3) : ((fy - 16 / 116) / 7.787)), 5);
                float z = (float)System.Math.Round(
                    zw * ((System.Math.Pow(fz, 3) > 0.008856) ? System.Math.Pow(fz, 3) : ((fz - 16 / 116) / 7.787)), 5);

                float r = x * 3.2406f - y * 1.5372f - z * 0.4986f;
                float g = -x * 0.9689f + y * 1.8758f + z * 0.0415f;
                float bValue = x * 0.0557f - y * 0.2040f + z * 1.0570f;

                r = r > 0.0031308f ? 1.055f * (float)System.Math.Pow(r, 1 / 2.4) - 0.055f : r * 12.92f;
                g = g > 0.0031308f ? 1.055f * (float)System.Math.Pow(g, 1 / 2.4) - 0.055f : g * 12.92f;
                bValue = bValue > 0.0031308f
                    ? 1.055f * (float)System.Math.Pow(bValue, 1 / 2.4) - 0.055f
                    : bValue * 12.92f;

                // r = (float)System.Math.Round(System.Math.Max(0, System.Math.Min(1, r)));
                // g = (float)System.Math.Round(System.Math.Max(0, System.Math.Min(1, g)));
                // bValue = (float)System.Math.Round(System.Math.Max(0, System.Math.Min(1, bValue)));

                return new Color(r, g, bValue);

            }

        }


        public static Color Greyscale(float brightness, float alpha = 1) =>
            new(brightness, brightness, brightness, alpha);

        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color MultiplyAlpha(this Color color, float k)
        {
            color.a *= k;
            return color;
        }





        #endregion

        #region Objects


        public static Object[] FindObjects(Type type)
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType(type, FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType(type);
#endif
        }

        public static T[] FindObjects<T>() where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>();
#endif
        }

        public static void Destroy(this Object r)
        {
            if (Application.isPlaying)
                Object.Destroy(r);
            else
                Object.DestroyImmediate(r);

        }

        public static void DestroyImmediate(this Object o) => Object.DestroyImmediate(o);





        #endregion

        #region GameObjects


        public static bool IsPrefab(this GameObject go) => go.scene.name == null || go.scene.name == go.name;

        public static Bounds GetBounds(this GameObject go, bool local = false)
        {
            Bounds bounds = default;

            foreach (var r in go.GetComponentsInChildren<MeshRenderer>())
            {
                var b = local ? r.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds : r.bounds;

                if (bounds == default)
                    bounds = b;
                else
                    bounds.Encapsulate(b);
            }
            
            foreach (var r in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var b = local ? r.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh.bounds : r.bounds;

                if (bounds == default)
                    bounds = b;
                else
                    bounds.Encapsulate(b);
            }


            foreach (var r in go.GetComponentsInChildren<Terrain>())
            {
                var b = local
                    ? new Bounds(r.terrainData.size / 2, r.terrainData.size)
                    : new Bounds(r.transform.position + r.terrainData.size / 2, r.terrainData.size);

                if (bounds == default)
                    bounds = b;
                else
                    bounds.Encapsulate(new Bounds(r.transform.position + r.terrainData.size / 2, r.terrainData.size));

            }

            foreach (var r in go.GetComponentsInChildren<RectTransform>())
            {
                var localBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(r);
                var worldBounds = new Bounds(r.TransformPoint(localBounds.center), r.TransformVector(localBounds.size));

                if (bounds == default)
                    bounds = worldBounds;
                else
                    bounds.Encapsulate(worldBounds);

            }


            if (bounds == default)
                bounds.center = go.transform.position;

            return bounds;
        }





        #endregion

        #region Text


        public static bool IsEmpty(this string s) => s == "";
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        public static bool IsLower(this char c) => System.Char.IsLower(c);
        public static bool IsUpper(this char c) => System.Char.IsUpper(c);
        public static bool IsDigit(this char c) => System.Char.IsDigit(c);
        public static bool IsLetter(this char c) => System.Char.IsLetter(c);
        public static bool IsWhitespace(this char c) => System.Char.IsWhiteSpace(c);

        public static char ToLower(this char c) => System.Char.ToLower(c);
        public static char ToUpper(this char c) => System.Char.ToUpper(c);



        public static string Decamelcase(this string s)
        {
            return Regex.Replace(Regex.Replace(s, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        public static string FormatVariableName(this string s, bool lowercaseFollowingWords = true)
        {
            return string.Join(" ", s.Decamelcase()
                .Split(' ')
                .Select(r =>
                    new[] { "", "and", "or", "with", "without", "by", "from" }.Contains(r.ToLower()) ||
                    (lowercaseFollowingWords && !s.Trim().StartsWith(r))
                        ? r.ToLower()
                        : r.Substring(0, 1).ToUpper() + r.Substring(1))).Trim(' ');
        }

        public static string Remove(this string s, string toRemove)
        {
            if (toRemove == "") return s;
            return s.Replace(toRemove, "");
        }






        #endregion

        #region Paths


        public static bool HasParentPath(this string path) => path.LastIndexOf('/') > 0;

        public static string GetParentPath(this string path) =>
            path.HasParentPath() ? path.Substring(0, path.LastIndexOf('/')) : "";

        public static string GetFilename(this string path, bool withExtension = false) =>
            withExtension ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);

        public static string GetExtension(this string path) => Path.GetExtension(path);


        public static string ToGlobalPath(this string localPath) =>
            Application.dataPath + "/" + localPath.Substring(0, localPath.Length - 1);

        public static string ToLocalPath(this string globalPath) =>
            "Assets" + globalPath.Replace(Application.dataPath, "");



        public static string CombinePath(this string p1, string p2, bool useBackslashOnWindows = false)
        {
            if (useBackslashOnWindows) // false by default because all paths in unity use forward slashes, even on Windows
                return Path.Combine(p1, p2);
            else
                return Path.Combine(p1, p2).Replace('\\', '/');

        }

        public static bool IsSubpathOf(this string path, string of) => path.StartsWith(of + "/") || of == "";

        public static string GetDirectory(this string pathOrDirectory)
        {
            var directory = pathOrDirectory.Contains('.')
                ? pathOrDirectory.Substring(0, pathOrDirectory.LastIndexOf('/'))
                : pathOrDirectory;

            if (directory.Contains('.'))
                directory = directory.Substring(0, directory.LastIndexOf('/'));

            return directory;

        }

        public static bool DirectoryExists(this string pathOrDirectory) =>
            Directory.Exists(pathOrDirectory.GetDirectory());

        public static string EnsureDirExists(this string pathOrDirectory) // todo to EnsureDirectoryExists
        {
            var directory = pathOrDirectory.GetDirectory();

            if (directory.HasParentPath() && !Directory.Exists(directory.GetParentPath()))
                EnsureDirExists(directory.GetParentPath());

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return pathOrDirectory;

        }



        public static string ClearDir(this string dir)
        {
            if (!Directory.Exists(dir)) return dir;

            var diri = new DirectoryInfo(dir);
            foreach (var r in diri.EnumerateFiles()) r.Delete();
            foreach (var r in diri.EnumerateDirectories()) r.Delete(true);

            return dir;
        }






#if UNITY_EDITOR

        public static string EnsurePathIsUnique(this string path)
        {
            if (!path.DirectoryExists()) return path;

            var s = AssetDatabase.GenerateUniqueAssetPath(path); // returns empty if parent dir doesnt exist 

            return s == "" ? path : s;

        }

        public static void EnsureDirExistsAndRevealInFinder(string dir)
        {
            EnsureDirExists(dir);
            UnityEditor.EditorUtility.OpenWithDefaultApp(dir);
        }

#endif



        #endregion

        #region AssetDatabase

#if UNITY_EDITOR

        public static AssetImporter GetImporter(this Object t) =>
            AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));

        public static string ToPath(this string guid) =>
            AssetDatabase.GUIDToAssetPath(guid); // returns empty string if not found

        public static List<string> ToPaths(this IEnumerable<string> guids) => guids.Select(r => r.ToPath()).ToList();


        public static string ToGuid(this string pathInProject) => AssetDatabase.AssetPathToGUID(pathInProject);

        public static List<string> ToGuids(this IEnumerable<string> pathsInProject) =>
            pathsInProject.Select(r => r.ToGuid()).ToList();

        public static string GetPath(this Object o) => AssetDatabase.GetAssetPath(o);
        public static string GetGuid(this Object o) => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o));

        public static string GetScriptPath(string scriptName) =>
            AssetDatabase.FindAssets("t: script " + scriptName, null).FirstOrDefault()?.ToPath() ??
            "scirpt not found"; // todonow to editorutils



#endif





        #endregion

        #region Serialization


        [System.Serializable]
        public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
        {
            [SerializeField] List<TKey> keys = new();
            [SerializeField] List<TValue> values = new();

            public void OnBeforeSerialize()
            {
                keys.Clear();
                values.Clear();

                foreach (KeyValuePair<TKey, TValue> kvp in this)
                {
                    keys.Add(kvp.Key);
                    values.Add(kvp.Value);
                }

            }

            public void OnAfterDeserialize()
            {
                this.Clear();

                for (int i = 0; i < keys.Count; i++)
                    this[keys[i]] = values[i];

            }

        }


#if UNITY_EDITOR

        public static object GetBoxedValue(this SerializedProperty p)
        {

#if UNITY_2022_1_OR_NEWER
            switch (p.propertyType)
            {
                case SerializedPropertyType.Integer:
                    switch (p.numericType)
                    {
                        case SerializedPropertyNumericType.Int8: return (sbyte)p.intValue;
                        case SerializedPropertyNumericType.UInt8: return (byte)p.uintValue;
                        case SerializedPropertyNumericType.Int16: return (short)p.intValue;
                        case SerializedPropertyNumericType.UInt16: return (ushort)p.uintValue;
                        case SerializedPropertyNumericType.Int32: return p.intValue;
                        case SerializedPropertyNumericType.UInt32: return p.uintValue;
                        case SerializedPropertyNumericType.Int64: return p.longValue;
                        case SerializedPropertyNumericType.UInt64: return p.ulongValue;
                        default: return p.intValue;

                    }

                case SerializedPropertyType.Float:
                    if (p.numericType == SerializedPropertyNumericType.Double)
                        return p.doubleValue;
                    else
                        return p.floatValue;

                case SerializedPropertyType.Hash128: return p.hash128Value;
                case SerializedPropertyType.Character: return (ushort)p.uintValue;
                case SerializedPropertyType.Gradient: return p.gradientValue;
                case SerializedPropertyType.ManagedReference: return p.managedReferenceValue;


            }
#endif

            switch (p.propertyType)
            {
                case SerializedPropertyType.Integer: return p.intValue;
                case SerializedPropertyType.Float: return p.floatValue;
                case SerializedPropertyType.Vector2: return p.vector2Value;
                case SerializedPropertyType.Vector3: return p.vector3Value;
                case SerializedPropertyType.Vector4: return p.vector4Value;
                case SerializedPropertyType.Vector2Int: return p.vector2IntValue;
                case SerializedPropertyType.Vector3Int: return p.vector3IntValue;
                case SerializedPropertyType.Quaternion: return p.quaternionValue;
                case SerializedPropertyType.Rect: return p.rectValue;
                case SerializedPropertyType.RectInt: return p.rectIntValue;
                case SerializedPropertyType.Bounds: return p.boundsValue;
                case SerializedPropertyType.BoundsInt: return p.boundsIntValue;
                case SerializedPropertyType.Enum: return p.enumValueIndex;
                case SerializedPropertyType.Boolean: return p.boolValue;
                case SerializedPropertyType.String: return p.stringValue;
                case SerializedPropertyType.Color: return p.colorValue;
                case SerializedPropertyType.ArraySize: return p.intValue;
                case SerializedPropertyType.Character: return (ushort)p.intValue;
                case SerializedPropertyType.AnimationCurve: return p.animationCurveValue;
                case SerializedPropertyType.ObjectReference: return p.objectReferenceValue;
                case SerializedPropertyType.ExposedReference: return p.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize: return p.intValue;
                case SerializedPropertyType.LayerMask: return (LayerMask)p.intValue;

            }


            return _noValue;

        }

        public static void SetBoxedValue(this SerializedProperty p, object value)
        {
            if (value == _noValue) return;

            try
            {

#if UNITY_2022_1_OR_NEWER
                switch (p.propertyType)
                {
                    case SerializedPropertyType.ArraySize:
                    case SerializedPropertyType.Integer:
                        if (p.numericType == SerializedPropertyNumericType.UInt64)
                            p.ulongValue = System.Convert.ToUInt64(value);
                        else
                            p.longValue = System.Convert.ToInt64(value);
                        return;

                    case SerializedPropertyType.Float:
                        if (p.numericType == SerializedPropertyNumericType.Double)
                            p.doubleValue = System.Convert.ToDouble(value);
                        else
                            p.floatValue = System.Convert.ToSingle(value);
                        return;

                    case SerializedPropertyType.Character:
                        p.uintValue = System.Convert.ToUInt16(value);
                        return;
                    case SerializedPropertyType.Gradient:
                        p.gradientValue = (Gradient)value;
                        return;
                    case SerializedPropertyType.Hash128:
                        p.hash128Value = (Hash128)value;
                        return;

                }
#endif

                switch (p.propertyType)
                {
                    case SerializedPropertyType.ArraySize:
                    case SerializedPropertyType.Integer:
                        p.intValue = System.Convert.ToInt32(value);
                        return;
                    case SerializedPropertyType.Float:
                        p.floatValue = System.Convert.ToSingle(value);
                        return;
                    case SerializedPropertyType.Vector2:
                        p.vector2Value = (Vector2)value;
                        return;
                    case SerializedPropertyType.Vector3:
                        p.vector3Value = (Vector3)value;
                        return;
                    case SerializedPropertyType.Vector4:
                        p.vector4Value = (Vector4)value;
                        return;
                    case SerializedPropertyType.Vector2Int:
                        p.vector2IntValue = (Vector2Int)value;
                        return;
                    case SerializedPropertyType.Vector3Int:
                        p.vector3IntValue = (Vector3Int)value;
                        return;
                    case SerializedPropertyType.Quaternion:
                        p.quaternionValue = (Quaternion)value;
                        return;
                    case SerializedPropertyType.Rect:
                        p.rectValue = (Rect)value;
                        return;
                    case SerializedPropertyType.RectInt:
                        p.rectIntValue = (RectInt)value;
                        return;
                    case SerializedPropertyType.Bounds:
                        p.boundsValue = (Bounds)value;
                        return;
                    case SerializedPropertyType.BoundsInt:
                        p.boundsIntValue = (BoundsInt)value;
                        return;
                    case SerializedPropertyType.String:
                        p.stringValue = (string)value;
                        return;
                    case SerializedPropertyType.Boolean:
                        p.boolValue = (bool)value;
                        return;
                    case SerializedPropertyType.Enum:
                        p.enumValueIndex = (int)value;
                        return;
                    case SerializedPropertyType.Color:
                        p.colorValue = (Color)value;
                        return;
                    case SerializedPropertyType.AnimationCurve:
                        p.animationCurveValue = (AnimationCurve)value;
                        return;
                    case SerializedPropertyType.ObjectReference:
                        p.objectReferenceValue = (UnityEngine.Object)value;
                        return;
                    case SerializedPropertyType.ExposedReference:
                        p.exposedReferenceValue = (UnityEngine.Object)value;
                        return;
                    case SerializedPropertyType.ManagedReference:
                        p.managedReferenceValue = value;
                        return;

                    case SerializedPropertyType.LayerMask:
                        try
                        {
                            p.intValue = ((LayerMask)value).value;
                            return;
                        }
                        catch (System.InvalidCastException)
                        {
                            p.intValue = System.Convert.ToInt32(value);
                            return;
                        }

                }

            }
            catch
            {
            }

        }

        static object _noValue = new();

#endif




        #endregion

        #region GlobalID

#if UNITY_EDITOR

        [System.Serializable]
        public struct GlobalID : System.IEquatable<GlobalID>
        {
            public Object GetObject() => GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalObjectId);
            public int GetObjectInstanceId() => GlobalObjectId.GlobalObjectIdentifierToInstanceIDSlow(globalObjectId);


            public int idType => globalObjectId.identifierType;
            public string guid => globalObjectId.assetGUID.ToString();
            public ulong fileId => globalObjectId.targetObjectId;
            public ulong prefabId => globalObjectId.targetPrefabId;

            public bool isNull => globalObjectId.identifierType == 0;
            public bool isAsset => globalObjectId.identifierType == 1;
            public bool isSceneObject => globalObjectId.identifierType == 2;

            public GlobalObjectId globalObjectId =>
                _globalObjectId.Equals(default) && globalObjectIdString != null &&
                GlobalObjectId.TryParse(globalObjectIdString, out var r)
                    ? _globalObjectId = r
                    : _globalObjectId;
            public GlobalObjectId _globalObjectId;

            public GlobalID(Object o) => globalObjectIdString =
                (_globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(o)).ToString();

            public GlobalID(string s) => globalObjectIdString = GlobalObjectId.TryParse(s, out _globalObjectId) ? s : s;

            public string globalObjectIdString;



            public bool Equals(GlobalID other) => this.globalObjectIdString.Equals(other.globalObjectIdString);

            public static bool operator ==(GlobalID a, GlobalID b) => a.Equals(b);
            public static bool operator !=(GlobalID a, GlobalID b) => !a.Equals(b);

            public override bool Equals(object other) => other is GlobalID otherglobalID && this.Equals(otherglobalID);
            public override int GetHashCode() => globalObjectIdString == null ? 0 : globalObjectIdString.GetHashCode();


            public override string ToString() => globalObjectIdString;




            public GlobalID UnpackForPrefab()
            {
                var unpackedFileId = (this.fileId ^ this.prefabId) & 0x7fffffffffffffff;

                var unpackedGId = new GlobalID($"GlobalObjectId_V1-{this.idType}-{this.guid}-{unpackedFileId}-0");

                return unpackedGId;

            }

        }

        public static GlobalID GetGlobalID(this Object o) => new(o);

        public static GlobalID[] GetGlobalIDs(this IEnumerable<int> instanceIds)
        {
            var unityGlobalIds = new GlobalObjectId[instanceIds.Count()];

            GlobalObjectId.GetGlobalObjectIdsSlow(instanceIds.ToArray(), unityGlobalIds);

            var globalIds = unityGlobalIds.Select(r => new GlobalID(r.ToString()));

            return globalIds.ToArray();

        }

        public static Object[] GetObjects(this IEnumerable<GlobalID> globalIDs)
        {
            var goids = globalIDs.Select(r => r.globalObjectId).ToArray();

            var objects = new Object[goids.Length];

            GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(goids, objects);

            return objects;

        }

        public static int[] GetObjectInstanceIds(this IEnumerable<GlobalID> globalIDs)
        {
            var goids = globalIDs.Select(r => r.globalObjectId).ToArray();

            var iids = new int[goids.Length];

            GlobalObjectId.GlobalObjectIdentifiersToInstanceIDsSlow(goids, iids);

            return iids;

        }


#endif




        #endregion

        #region Editor

#if UNITY_EDITOR


        public static class EditorUtils
        {

            public static void OpenFolder(string path)
            {
                var folder = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

                var t = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
                var w = (EditorWindow)t.GetField("s_LastInteractedProjectBrowser").GetValue(null);

                var m_ListAreaState = t.GetField("m_ListAreaState", maxBindingFlags).GetValue(w);

                m_ListAreaState.GetType().GetField("m_SelectedInstanceIDs")
                    .SetValue(m_ListAreaState, new List<int> { folder.GetInstanceID() });

                t.GetMethod("OpenSelectedFolders", maxBindingFlags).Invoke(null, null);

            }

            public static void PingObject(Object o, bool select = false, bool focusProjectWindow = true)
            {
                if (select)
                {
                    Selection.activeObject = null;
                    Selection.activeObject = o;
                }

                if (focusProjectWindow) EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(o);

            }

            public static void PingObject(string guid, bool select = false, bool focusProjectWindow = true) =>
                PingObject(AssetDatabase.LoadAssetAtPath<Object>(guid.ToPath()));

            public static EditorWindow OpenObjectPicker<T>(Object obj = null, bool allowSceneObjects = false,
                string searchFilter = "", int controlID = 0) where T : Object
            {
                EditorGUIUtility.ShowObjectPicker<T>(obj, allowSceneObjects, searchFilter, controlID);

                return Resources.FindObjectsOfTypeAll(typeof(Editor).Assembly.GetType("UnityEditor.ObjectSelector"))
                    .FirstOrDefault() as EditorWindow;

            }

            public static EditorWindow OpenColorPicker(System.Action<Color> colorChangedCallback, Color color,
                bool showAlpha = true, bool hdr = false)
            {
                typeof(Editor).Assembly.GetType("UnityEditor.ColorPicker")
                    .InvokeMethod("Show", colorChangedCallback, color, showAlpha, hdr);

                return typeof(Editor).Assembly.GetType("UnityEditor.ColorPicker")
                    .GetPropertyValue<EditorWindow>("instance");

            }



            public static bool CheckUnityVersion(string versionQuery)
            {
                if (versionQueryCache.TryGetValue(versionQuery, out var cachedResult)) return cachedResult;

                if (versionQuery.Any(r =>
                        r.IsLetter() && !versionQuery.EndsWith(" or older") && !versionQuery.EndsWith(" or newer")))
                    throw new System.ArgumentException("Invalid unity version query");




                var curVersion = new string(Application.unityVersion.TakeWhile(r => !r.IsLetter()).ToArray());

                var curMajor = int.Parse(curVersion.Split('.')[0]);
                var curMinor = int.Parse(curVersion.Split('.')[1]);
                var curPatch = int.Parse(curVersion.Split('.')[2]);





                var givenVersion = new string(versionQuery.TakeWhile(r => !r.IsWhitespace()).ToArray());

                var isMinorGiven = givenVersion.Count(r => r == '.') >= 1;
                var isPatchGiven = givenVersion.Count(r => r == '.') >= 2;

                var givenMajor = int.Parse(givenVersion.Split('.')[0]);
                var givenMinor = isMinorGiven ? int.Parse(givenVersion.Split('.')[1]) : 0;
                var givenPatch = isPatchGiven ? int.Parse(givenVersion.Split('.')[2]) : 0;






                var curVersionCanBeNewer = versionQuery.Contains("or newer");
                var curVersionCanBeOlder = versionQuery.Contains("or older");


                if (curMajor > givenMajor) return versionQueryCache[versionQuery] = curVersionCanBeNewer;
                if (curMajor < givenMajor) return versionQueryCache[versionQuery] = curVersionCanBeOlder;

                if (!isMinorGiven) return versionQueryCache[versionQuery] = true;


                if (curMinor > givenMinor) return versionQueryCache[versionQuery] = curVersionCanBeNewer;
                if (curMinor < givenMinor) return versionQueryCache[versionQuery] = curVersionCanBeOlder;

                if (!isPatchGiven) return versionQueryCache[versionQuery] = true;


                if (curPatch > givenPatch) return versionQueryCache[versionQuery] = curVersionCanBeNewer;
                if (curPatch < givenPatch) return versionQueryCache[versionQuery] = curVersionCanBeOlder;

                return versionQueryCache[versionQuery] = true;




                // query examples:
                // 
                // "2022.3.5 or newer"
                // "2022.3.5 or older"
                // "2022.3 or older"
                // "2022.3"
                // "2022"

            }

            static Dictionary<string, bool> versionQueryCache = new();



            public static void SetSymbolDefinedInAsmdef(string asmdefName, string symbol, bool defined)
            {
                var isDefined = IsSymbolDefinedInAsmdef(asmdefName, symbol);
                var shouldBeDefined = defined;

                if (shouldBeDefined && !isDefined)
                    DefineSymbolInAsmdef(asmdefName, symbol);

                if (!shouldBeDefined && isDefined)
                    UndefineSymbolInAsmdef(asmdefName, symbol);

            }

            public static bool IsSymbolDefinedInAsmdef(string asmdefName, string symbol)
            {
                var path = AssetDatabase.FindAssets("t: asmdef " + asmdefName, null).First().ToPath();
                var importer = AssetImporter.GetAtPath(path);

                var editorType = typeof(Editor).Assembly.GetType("UnityEditor.AssemblyDefinitionImporterInspector");
                var editor = Editor.CreateEditor(importer, editorType);

                var state = editor.GetFieldValue<Object[]>("m_ExtraDataTargets").First();


                var definesList = state.GetFieldValue<IList>("versionDefines");
                var isSymbolDefined = Enumerable.Range(0, definesList.Count)
                    .Any(i => definesList[i].GetFieldValue<string>("define") == symbol);


                Object.DestroyImmediate(editor);

                return isSymbolDefined;

            }

            static void DefineSymbolInAsmdef(string asmdefName, string symbol)
            {
                var path = AssetDatabase.FindAssets("t: asmdef " + asmdefName, null).First().ToPath();
                var importer = AssetImporter.GetAtPath(path);

                var editorType = typeof(Editor).Assembly.GetType("UnityEditor.AssemblyDefinitionImporterInspector");
                var editor = Editor.CreateEditor(importer, editorType);

                var state = editor.GetFieldValue<Object[]>("m_ExtraDataTargets").First();


                var definesList = state.GetFieldValue<IList>("versionDefines");

                var defineType = definesList.GetType().GenericTypeArguments[0];
                var newDefine = System.Activator.CreateInstance(defineType);

                newDefine.SetFieldValue("name", "Unity");
                newDefine.SetFieldValue("define", symbol);

                definesList.Add(newDefine);


                editor.InvokeMethod("Apply");

                Object.DestroyImmediate(editor);

            }

            static void UndefineSymbolInAsmdef(string asmdefName, string symbol)
            {
                var path = AssetDatabase.FindAssets("t: asmdef " + asmdefName, null).First().ToPath();
                var importer = AssetImporter.GetAtPath(path);

                var editorType = typeof(Editor).Assembly.GetType("UnityEditor.AssemblyDefinitionImporterInspector");
                var editor = Editor.CreateEditor(importer, editorType);

                var state = editor.GetFieldValue<Object[]>("m_ExtraDataTargets").First();


                var definesList = state.GetFieldValue<IList>("versionDefines");

                var defineIndex = Enumerable.Range(0, definesList.Count)
                    .First(i => definesList[i].GetFieldValue<string>("define") == symbol);

                definesList.RemoveAt(defineIndex);


                editor.InvokeMethod("Apply");

                Object.DestroyImmediate(editor);

            }




            public static int GetCurrendUndoGroupIndex()
            {
                var args = new object[] { _dummyList, 0 };

                typeof(Undo).GetMethodInfo("GetRecords", typeof(List<string>), typeof(int).MakeByRefType())
                    .Invoke(null, args);


                return (int)args[1];

            }

            static List<string> _dummyList = new();





            public static void Hide(string path)
            {
                if (IsHidden(path)) return;

                if (File.Exists(path))
                    File.Move(path, path + "~");


                path += ".meta";
                if (File.Exists(path))
                    File.Move(path, path + "~");
            }

            public static void Unhide(string path)
            {
                if (!IsHidden(path)) return;
                if (path.EndsWith("~")) path = path.Substring(0, path.Length - 1);

                if (File.Exists(path + "~"))
                    File.Move(path + "~", path);

                path += ".meta";
                if (File.Exists(path + "~"))
                    File.Move(path + "~", path);
            }

            public static bool IsHidden(string path) => path.EndsWith("~") || File.Exists(path + "~");


            public static void CopyDirectoryDeep(string sourcePath, string destinationPath)
            {
                CopyDirectoryRecursively(sourcePath, destinationPath);

                var metas = GetFilesRecursively(destinationPath, (f) => f.EndsWith(".meta"));
                var guidTable = new List<(string originalGuid, string newGuid)>();

                foreach (string meta in metas)
                {
                    StreamReader file = new(meta);
                    file.ReadLine();
                    string guidLine = file.ReadLine();
                    file.Close();
                    string originalGuid = guidLine.Substring(6, guidLine.Length - 6);
                    string newGuid = GUID.Generate().ToString().Replace("-", "");
                    guidTable.Add((originalGuid, newGuid));
                }

                var allFiles = GetFilesRecursively(destinationPath);

                foreach (string fileToModify in allFiles)
                {
                    string content = File.ReadAllText(fileToModify);

                    foreach (var guidPair in guidTable)
                    {
                        content = content.Replace(guidPair.originalGuid, guidPair.newGuid);
                    }

                    File.WriteAllText(fileToModify, content);
                }

                AssetDatabase.Refresh();
            }

            private static void CopyDirectoryRecursively(string sourceDirName, string destDirName)
            {
                DirectoryInfo dir = new(sourceDirName);

                DirectoryInfo[] dirs = dir.GetDirectories();

                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }

                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectoryRecursively(subdir.FullName, temppath);
                }
            }

            private static List<string> GetFilesRecursively(string path, System.Func<string, bool> criteria = null,
                List<string> files = null)
            {
                if (files == null)
                {
                    files = new List<string>();
                }

                files.AddRange(Directory.GetFiles(path).Where(f => criteria == null || criteria(f)));

                foreach (string directory in Directory.GetDirectories(path))
                {
                    GetFilesRecursively(directory, criteria, files);
                }

                return files;
            }





            // for non-extension methods

        }


        public static class EditorPrefsCached
        {
            public static int GetInt(string key, int defaultValue = 0)
            {
                if (ints_byKey.ContainsKey(key))
                    return ints_byKey[key];
                else
                    return ints_byKey[key] = EditorPrefs.GetInt(key, defaultValue);

            }

            public static bool GetBool(string key, bool defaultValue = false)
            {
                if (bools_byKey.ContainsKey(key))
                    return bools_byKey[key];
                else
                    return bools_byKey[key] = EditorPrefs.GetBool(key, defaultValue);

            }

            public static float GetFloat(string key, float defaultValue = 0)
            {
                if (floats_byKey.ContainsKey(key))
                    return floats_byKey[key];
                else
                    return floats_byKey[key] = EditorPrefs.GetFloat(key, defaultValue);

            }

            public static string GetString(string key, string defaultValue = "")
            {
                if (strings_byKey.ContainsKey(key))
                    return strings_byKey[key];
                else
                    return strings_byKey[key] = EditorPrefs.GetString(key, defaultValue);

            }

            public static void SetInt(string key, int value)
            {
                ints_byKey[key] = value;

                EditorPrefs.SetInt(key, value);

            }

            public static void SetBool(string key, bool value)
            {
                bools_byKey[key] = value;

                EditorPrefs.SetBool(key, value);

            }

            public static void SetFloat(string key, float value)
            {
                floats_byKey[key] = value;

                EditorPrefs.SetFloat(key, value);

            }

            public static void SetString(string key, string value)
            {
                strings_byKey[key] = value;

                EditorPrefs.SetString(key, value);

            }


            static Dictionary<string, int> ints_byKey = new();
            static Dictionary<string, bool> bools_byKey = new();
            static Dictionary<string, float> floats_byKey = new();
            static Dictionary<string, string> strings_byKey = new();

        }

        public static class ProjectPrefs
        {
            public static int GetInt(string key, int defaultValue = 0) =>
                EditorPrefsCached.GetInt(key + projectId, defaultValue);

            public static bool GetBool(string key, bool defaultValue = false) =>
                EditorPrefsCached.GetBool(key + projectId, defaultValue);

            public static float GetFloat(string key, float defaultValue = 0) =>
                EditorPrefsCached.GetFloat(key + projectId, defaultValue);

            public static string GetString(string key, string defaultValue = "") =>
                EditorPrefsCached.GetString(key + projectId, defaultValue);

            public static void SetInt(string key, int value) => EditorPrefsCached.SetInt(key + projectId, value);
            public static void SetBool(string key, bool value) => EditorPrefsCached.SetBool(key + projectId, value);
            public static void SetFloat(string key, float value) => EditorPrefsCached.SetFloat(key + projectId, value);

            public static void SetString(string key, string value) =>
                EditorPrefsCached.SetString(key + projectId, value);



            public static bool HasKey(string key) => EditorPrefs.HasKey(key + projectId);
            public static void DeleteKey(string key) => EditorPrefs.DeleteKey(key + projectId);



            public static int projectId => PlayerSettings.productGUID.GetHashCode();

        }



        public static void RecordUndo(this Object o, string operationName = "") => Undo.RecordObject(o, operationName);
        public static void Dirty(this Object o) => UnityEditor.EditorUtility.SetDirty(o);
        public static void Save(this Object o) => AssetDatabase.SaveAssetIfDirty(o);



        public static void SelectInInspector(this Object[] objects, bool frameInHierarchy = false,
            bool frameInProject = false)
        {
            void setHierarchyLocked(bool isLocked) => allHierarchies.ForEach(r =>
                r?.GetMemberValue("m_SceneHierarchy")?.SetMemberValue("m_RectSelectInProgress", isLocked));

            void setProjectLocked(bool isLocked) =>
                allProjectBrowsers.ForEach(r => r?.SetMemberValue("m_InternalSelectionChange", isLocked));


            if (!frameInHierarchy) setHierarchyLocked(true);
            if (!frameInProject) setProjectLocked(true);

            Selection.objects = objects?.ToArray();

            if (!frameInHierarchy) EditorApplication.delayCall += () => setHierarchyLocked(false);
            if (!frameInProject) EditorApplication.delayCall += () => setProjectLocked(false);

        }

        public static void SelectInInspector(this Object obj, bool frameInHierarchy = false,
            bool frameInProject = false) => new[] { obj }.SelectInInspector(frameInHierarchy, frameInProject);

        static IEnumerable<EditorWindow> allHierarchies => _allHierarchies ??= typeof(Editor).Assembly
            .GetType("UnityEditor.SceneHierarchyWindow").GetFieldValue<IList>("s_SceneHierarchyWindows")
            .Cast<EditorWindow>();
        static IEnumerable<EditorWindow> _allHierarchies;

        static IEnumerable<EditorWindow> allProjectBrowsers => _allProjectBrowsers ??= typeof(Editor).Assembly
            .GetType("UnityEditor.ProjectBrowser").GetFieldValue<IList>("s_ProjectBrowsers").Cast<EditorWindow>();
        static IEnumerable<EditorWindow> _allProjectBrowsers;



        public static void MoveTo(this EditorWindow window, Vector2 position, bool ensureFitsOnScreen = true)
        {
            if (!ensureFitsOnScreen)
            {
                window.position = window.position.SetPos(position);
                return;
            }

            var windowRect = window.position;
            var unityWindowRect = EditorGUIUtility.GetMainWindowPosition();

            position.x = position.x.Max(unityWindowRect.position.x);
            position.y = position.y.Max(unityWindowRect.position.y);

            position.x = position.x.Min(unityWindowRect.xMax - windowRect.width);
            position.y = position.y.Min(unityWindowRect.yMax - windowRect.height);

            window.position = windowRect.SetPos(position);

        }



#endif

        #endregion

    }
}