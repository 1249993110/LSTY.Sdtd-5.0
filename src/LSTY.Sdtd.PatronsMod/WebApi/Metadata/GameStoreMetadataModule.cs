using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Functions;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    public class GameStoreMetadataModule : MetadataModule<PathItem>
    {
        public GameStoreMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<T_Goods>();
            modelCatalog.AddModel<IEnumerable<T_Goods>>();
            modelCatalog.AddModel<T_ContentTypes>();
            modelCatalog.AddModel<IEnumerable<T_ContentTypes>>();
            modelCatalog.AddModel<GameStoreConfigViewModel>();
            modelCatalog.AddModel<GoodsViewModelBase>();
            modelCatalog.AddModel<GoodsViewModel>();
            modelCatalog.AddModel<DeleteQueryParamOfString>();

            Describe["RetrieveGameStoreConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveGameStoreConfig")
                            .Tag("GameStore")
                            .Summary("获取游戏商店配置")
                            .Description("")
                            .Response(r => r.Schema<GameStoreConfigViewModel>().Description("The config of GameStore"))));

            Describe["UpdateGameStoreConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateGameStoreConfig")
                            .Tag("GameStore")
                            .Summary("更新游戏商店配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(GameStoreConfigViewModel)).Schema<GameStoreConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_GameStore"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveAvailableVariables_GameStore")
                            .Tag("GameStore")
                            .Summary("获取可用变量")
                            .Description("Get the availableVariables")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
            
            Describe["CreateGoods"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("CreateGoods")
                            .Tag("GameStore")
                            .Summary("创建商品")
                            .BodyParameter(p => p.Description("A object").Name(nameof(GoodsViewModelBase)).Schema<GoodsViewModelBase>())
                            .Description("")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveGoods"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveGoods")
                            .Tag("GameStore")
                            .Summary("获取商品")
                            .Description("returns all goods")
                            .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<T_Goods>>(modelCatalog).Description("Succeeded or failed"))));

            Describe["UpdateGoods"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateGoods")
                            .Tag("GameStore")
                            .Summary("更新商品")
                            .Description("")
                            .BodyParameter(p => p.Description("A object").Name(nameof(GoodsViewModel)).Schema<GoodsViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["DeleteGoods"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("DeleteGoods")
                            .Tag("GameStore")
                            .Summary("删除商品")
                            .BodyParameter(p => p.Description("A array").Name(nameof(DeleteQueryParamOfString)).Schema<DeleteQueryParamOfString>())
                            .Description("Delete goods by ids")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveContentTypes"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveContentTypes")
                            .Tag("GameStore")
                            .Summary("获取商品奖品内容类型")
                            .Description("returns all content types")
                            .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<T_ContentTypes>>(modelCatalog).Description("Succeeded or failed"))));
        }
    }
}
