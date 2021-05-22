using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    public class ChatLogMetaDataModule : MetadataModule<PathItem>
    {
        public ChatLogMetaDataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<ChatLogQueryParam>();
            modelCatalog.AddModel<T_ChatLog>();

            Describe["RetrieveChatLogBySteamId"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveChatLogBySteamId")
                            .Tag("ChatLog")
                            .Summary("通过 steamId 获取聊天记录")
                            .Parameter(new Parameter() { Name = "steamId", In = ParameterIn.Query, Required=true })
                            .Description("If the parameter steamId is null then return a failed result, otherwise returns chatLog")
                            .Response(r => r.Schema<T_ChatLog>().Description("Chat logs"))));

            Describe["RetrieveChatLogByEntityId"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveChatLogByEntityId")
                            .Tag("ChatLog")
                            .Summary("通过 entityId 获取聊天记录")
                            .Parameter(new Parameter() { Name = "entityId", In = ParameterIn.Query, Required = true })
                            .Description("If the parameter entityId is null then return a failed result, otherwise returns chatLog")
                            .Response(r => r.Schema<T_ChatLog>().Description("Chat logs"))));

            Describe["RetrieveChatLogByDateTime"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveChatLogByDateTime")
                            .Tag("ChatLog")
                            .Summary("通过时间段获取聊天记录")
                            .Parameter(new Parameter() { Name = "startDateTime", In = ParameterIn.Query, Required = true })
                            .Parameter(new Parameter() { Name = "endDateTime", In = ParameterIn.Query, Required = true })
                            .Description("If the parameter startDateTime or endDateTime is null then return a failed result, otherwise returns chatLog")
                            .Response(r => r.Schema<T_ChatLog>().Description("Chat logs"))));

            Describe["RetrieveChatLogPaged"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveChatLogPaged")
                            .Tag("ChatLog")
                            .Summary("通过分页参数获取聊天记录")
                            .BodyParameter(p => p.Description("Query params").Name(nameof(ChatLogQueryParam)).Schema<ChatLogQueryParam>())
                            .Description("If the parameter steamId is optional")
                            .Response(r => r.Schema<T_ChatLog>().Description("Chat logs"))));

        }
    }
}
