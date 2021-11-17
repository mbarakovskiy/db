using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        private readonly IMongoCollection<GameEntity> gameCollection;

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return FindById(game.Id);
        }

        public GameEntity FindById(Guid gameId)
        {
            var game = gameCollection
                .Find(x => x.Id == gameId)
                .FirstOrDefault();
            return game;
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(x => x.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            //TODO: Используй Find и Limit
            var games = gameCollection
                .Find(x => x.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
            return new List<GameEntity>(games);
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            //TODO: Для проверки успешности используй IsAcknowledged и ModifiedCount из результата
            var foundedGame = FindById(game.Id);
            if (foundedGame == null || foundedGame.Status != GameStatus.WaitingToStart) return false;
            var newGame = new GameEntity(game.Id, GameStatus.Playing, game.TurnsCount, game.CurrentTurnIndex, game.Players.ToList());
            gameCollection.ReplaceOne(x => x.Id == game.Id, newGame);
            return true;

        }
    }
}