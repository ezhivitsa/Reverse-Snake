using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using Assets.ReverseSnake.Scripts.Attributes;

namespace Assets.ReverseSnake.Scripts.Extensions
{
    public static class AttributeExtensions
    {
        public static T GetAttribute<T>(this object value) where T : Attribute
        {
            if (value == null)
                return null;

            var fi = value.GetType().GetField(value.ToString());

            return fi.GetAttribute<T>();
        }

        public static T GetAttribute<T>(this MemberInfo mi) where T : Attribute
        {
            if (mi == null) return null;
            var attributes = (T[])mi.GetCustomAttributes(typeof(T), false);

            return attributes.SingleOrDefault(x => x.GetType() == typeof(T));
        }

        public static string GetDescription(this object value)
        {
            if (value == null)
                return null;

            var mi = value as MemberInfo;
            var isClassMember = mi != null;

            var attribute = isClassMember
                ? mi.GetAttribute<DescriptionAttribute>()
                : value.GetAttribute<DescriptionAttribute>();

            string description;
            if (attribute == null)
            {
                var valueStr = isClassMember ? mi.Name : value.ToString();
                description = valueStr.SeparateWords();
            }
            else
                description = attribute.Description;

            return description;
        }

        public static float GetStartProbability(this object value)
        {
            var attribute = GetProbabilitiesAttribute(value);
            return attribute != null ? attribute.StartProbability : 0f;
        }

        public static float GetEndProbability(this object value)
        {
            var attribute = GetProbabilitiesAttribute(value);
            return attribute != null ? attribute.EndProbability : 0f;
        }

        public static float GetIntermediateProbability(this object value)
        {
            var attribute = GetProbabilitiesAttribute(value);
            return attribute != null ? attribute.IntermediateProbability : 0f;
        }

        public static string GetTextureName(this object value)
        {
            var attribute = GetAttribute<TextureAttribute>(value);
            return attribute.Texture;
        }

        private static ProbabilitiesAttribute GetProbabilitiesAttribute(object value)
        {
            if (value == null)
                return null;

            var mi = value as MemberInfo;
            var isClassMember = mi != null;

            var attribute = isClassMember
                ? mi.GetAttribute<ProbabilitiesAttribute>()
                : value.GetAttribute<ProbabilitiesAttribute>();

            return attribute;
        }
    }
}
