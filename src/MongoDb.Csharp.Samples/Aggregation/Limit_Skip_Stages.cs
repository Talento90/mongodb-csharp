﻿using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDb.Csharp.Samples.Core;
using MongoDb.Csharp.Samples.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDb.Csharp.Samples.Aggregation
{
    public class LimitSkipStages : RunnableSample, IRunnableSample
    {
        public override Core.Samples Sample => Core.Samples.Aggregation_Stages_Limit_Skip;
        protected override void Init()
        {
            // Create a mongodb client
            Client = new MongoClient(Utils.DefaultConnectionString);
            Utils.DropDatabase(Client, Databases.Persons);
        }

        public async Task Run()
        {
            await LimitSkipSamples();
        }

        private async Task LimitSkipSamples()
        {
            var usersCollectionName = "users";
            var personsDatabase = Client.GetDatabase(Databases.Persons);
            var usersCollection = personsDatabase.GetCollection<User>(usersCollectionName);
            var usersQueryableCollection = personsDatabase.GetCollection<User>(usersCollectionName).AsQueryable();
            var usersBsonCollection = personsDatabase.GetCollection<BsonDocument>(usersCollectionName);
            #region Prepare data

            await usersCollection.InsertManyAsync(RandomData.GenerateUsers(100));

            var skipSize = 3;
            var limitSize = 3;
            #endregion

            #region Typed

            #region Top Level

            // In find order doesn't matter but in pipelines it does!
            
            // Order users by their birth date, older persons first

            var topLevelProjection = Builders<User>.Projection
                .Exclude(u => u.Id)
                .Include(u => u.UserName)
                .Include(u => u.DateOfBirth);

            var topLevelProjectionResults = await usersCollection.Find(Builders<User>.Filter.Empty)
                .Project(topLevelProjection)
                .SortBy(u => u.DateOfBirth)
                .Skip(skipSize)
                .Limit(limitSize)
                .ToListAsync();

            foreach (var topLevelProjectionResult in topLevelProjectionResults)
            {
                Utils.Log(topLevelProjectionResult.ToJson());
            }

            var linqTopLevelResults = await usersQueryableCollection
                .Select(u => new { u.UserName, u.DateOfBirth })
                .OrderBy(u => u.DateOfBirth)
                .Skip(skipSize)
                .Take(limitSize)
                .ToListAsync();

            #endregion



            #endregion

            #region BsonDocument commands

            var bsonTopLevelProjection = Builders<BsonDocument>.Projection
                .Exclude("_id")
                .Include("userName")
                .Include("dateOfBirth");

            var bsonTopLevelProjectionResults = await usersBsonCollection.Find(Builders<BsonDocument>.Filter.Empty)
                .Project(bsonTopLevelProjection)
                .SortBy(doc => doc["dateOfBirth"])
                .Skip(skipSize)
                .Limit(limitSize)
                .ToListAsync();

            #endregion


            #region Shell commands

#if false
            db.users.aggregate([
                { "$project" : { _id: 0, userName: 1, dateOfBirth: 1 } },
                { "$sort" : { dateOfBirth: 1 } },
                { "$skip":  3 },
                { "$limit": 3 }
            ])
#endif

            #endregion

        }
    }
}