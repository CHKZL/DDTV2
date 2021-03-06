﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDTVLiveRecWebServer.鉴权
{
    public class Authentication
    {
        public static 鉴权返回结果 API接口鉴权(HttpContext context,string cmd,bool 是否缺少关键参数=false)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in context.Request.Form)
            {
                dic.Add(item.Key, item.Value);
                if(string.IsNullOrEmpty(item.Value))
                {
                    return new 鉴权返回结果()
                    {
                        鉴权结果 = false,
                        鉴权返回消息 = "参数不能为空"
                    };
                }
            }
            if (dic.ContainsKey("token"))
            {
                return new 鉴权返回结果()
                {
                    鉴权结果 = false,
                    鉴权返回消息= "存在不应该存在的的参数"
                };
            }
            if (!dic.ContainsKey("sig")|| !dic.ContainsKey("time")|| !dic.ContainsKey("cmd")|| 是否缺少关键参数)
            {
                return new 鉴权返回结果()
                {
                    鉴权结果 = false,
                    鉴权返回消息 = "缺少必要参数"
                };
            }
            if(dic["cmd"]!=cmd)
            {
                return new 鉴权返回结果()
                {
                    鉴权结果 = false,
                    鉴权返回消息 = "参数不正确"
                };
            }
            dic.Add("token",Auxiliary.MMPU.ApiToken);
            var dicSort = from objDic in dic orderby objDic.Key ascending select objDic;
            List<List<string>> LS = new List<List<string>>();
            foreach (KeyValuePair<string, string> kvp in dicSort)
            {
                if (kvp.Key != "sig")
                    LS.Add(new List<string>() { kvp.Key, kvp.Value });
            }
            string 加密字符串 = LS[0][0] + "=" + LS[0][1];
            for (int i =1;i< LS.Count;i++)
            {
                加密字符串+="&"+ LS[i][0] + "=" + LS[i][1];
            }
            string 服务器sig = Auxiliary.Encryption.SHA1_Encrypt(加密字符串);
            Console.WriteLine(服务器sig);
            if(服务器sig== dic["sig"])
            {
                return new 鉴权返回结果()
                {
                    鉴权结果 = true,
                    鉴权返回消息 = "成功"
                };
            }
            else
            {
                return new 鉴权返回结果()
                {
                    鉴权结果 = false,
                    鉴权返回消息 = "sig校验失败"
                };
            }
        }
        public class 鉴权返回结果
        {
            public bool 鉴权结果 { set; get; }
            public string 鉴权返回消息 { set; get; }
        }
    }
}