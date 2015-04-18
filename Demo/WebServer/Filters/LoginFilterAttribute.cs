﻿using NetworkSocket.WebSocket;
using NetworkSocket.WebSocket.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Filters
{
    /// <summary>
    /// 登录过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LoginFilterAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(ActionContext filterContext)
        {
            var valid = filterContext.Client.TagData.TryGet<bool>("Logined");
            if (valid == false)
            {
                // 直接关闭客户端的连接
                // filterContext.Client.NormalClose(CloseReasons.NormalClosure);

                // 以异常方式提示客户端
                throw new Exception("未登录就尝试请求其它服务");
            }
        }
    }
}