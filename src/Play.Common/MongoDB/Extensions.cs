using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));//restore as string//
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));// z datetime bedzie stored as a string//
//SINGLETON define an singleton object that represents a mongo database thats gonna be injectet into items repository - constructing mongo clientt & mongo database before registring it in the container//
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
        });

        return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) 
            where T: IEntity
         {
            services.AddSingleton<IRepository<T>>(ServiceProvider => 
    {
        var database = ServiceProvider.GetService<IMongoDatabase>();
        return new MongoRepository<T>(database, collectionName);
    });

    return services;
    
       }
    }
}