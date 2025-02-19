using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            userCollection.Indexes.CreateOne(new BsonDocument("Login", 1), new CreateIndexOptions { Unique = true });
        }

        public UserEntity Insert(UserEntity user)
        {
            //TODO: Ищи в документации InsertXXX.
            userCollection.InsertOne(user);
            
            return FindById(user.Id);
        }

        public UserEntity FindById(Guid id)
        {
            //TODO: Ищи в документации FindXXX
            var user = userCollection
                .Find(x => x.Id == id)
                .FirstOrDefault();
            return user;
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection
                .Find(x => x.Login == login)
                .FirstOrDefault();
            if (user == null)
            {
                user = new UserEntity { Login = login };
                Insert(user);
            }

            return user;
        }

        public void Update(UserEntity user)
        {
            //TODO: Ищи в документации ReplaceXXX
            userCollection.ReplaceOne(x => x.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(x => x.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            //TODO: Тебе понадобятся SortBy, Skip и Limit
            var filter = new BsonDocument();
            var users = userCollection
                .Find(filter)
                .SortBy(x => x.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(users, userCollection.CountDocuments(filter), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}