﻿using NetworkSocket.Reflection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace NetworkSocket.Util.Converts
{
    /// <summary>
    /// 表示动态类型转换单元
    /// </summary>
    public class DynamicObjectConvert : IConvert
    {
        /// <summary>
        /// 转换器实例
        /// </summary>
        public Converter Converter { get; set; }

        /// <summary>
        /// 下一个转换单元
        /// </summary>
        public IConvert NextConvert { get; set; }

        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType)
        {
            var dynamicObject = value as DynamicObject;
            if (dynamicObject == null)
            {
                return this.NextConvert.Convert(value, targetType);
            }

            var instance = Activator.CreateInstance(targetType);
            var setters = Property.GetProperties(targetType);

            foreach (var set in setters)
            {
                if (set.Info.CanWrite == false)
                {
                    continue;
                }

                object targetValue;
                if (this.TryGetValue(dynamicObject, set.Name, out targetValue) == true)
                {
                    targetValue = this.Converter.Convert(targetValue, set.Info.PropertyType);
                    set.SetValue(instance, targetValue);
                }
            }

            return instance;
        }

        /// <summary>
        /// 获取动态类型的值
        /// </summary>
        /// <param name="dynamicObject">实例</param>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private bool TryGetValue(DynamicObject dynamicObject, string key, out object value)
        {
            var keys = dynamicObject.GetDynamicMemberNames();
            key = keys.FirstOrDefault(item => string.Equals(item, key, StringComparison.OrdinalIgnoreCase));

            if (key != null)
            {
                return dynamicObject.TryGetMember(new KeyBinder(key, false), out value);
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 表示键的信息获取绑定
        /// </summary>
        private class KeyBinder : GetMemberBinder
        {
            /// <summary>
            /// 键的信息获取绑定
            /// </summary>
            /// <param name="key">键名</param>
            /// <param name="ignoreCase">是否忽略大小写</param>
            public KeyBinder(string key, bool ignoreCase)
                : base(key, ignoreCase)
            {
            }

            /// <summary>
            /// 在派生类中重写时，如果无法绑定目标动态对象，则执行动态获取成员操作的绑定
            /// </summary>
            /// <param name="target"></param>
            /// <param name="errorSuggestion"></param>
            /// <returns></returns>
            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }
    }
}
