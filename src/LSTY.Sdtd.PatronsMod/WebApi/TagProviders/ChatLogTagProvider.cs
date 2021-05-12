﻿using Nancy.Swagger.Services;
using Swagger.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.TagProviders
{
    public class ChatLogTagProvider : ISwaggerTagProvider
    {
        public Tag GetTag()
        {
            return new Tag()
            {
                Description = "聊天记录",
                Name = "ChatLog"
            };
        }
    }
}
