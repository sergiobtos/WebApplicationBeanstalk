using System;
using System.Collections.Generic;
using System.Threading;
using WebApplicationBeanstalk.Models;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace WebApplicationBeanstalk.Service
{
    public class AWSServices
    {
        AmazonDynamoDBClient Client;
        DynamoDBContext Context;
        string Tmp = AppDomain.CurrentDomain.BaseDirectory;
        RegionEndpoint BucketRegion = RegionEndpoint.USWest1;

        public AWSServices()
        {
            Client = new AmazonDynamoDBClient(BucketRegion);
            Context = new DynamoDBContext(Client);
            CreateTable();
        }
        public async Task<User> Register(User user)
        {
            await Context.SaveAsync(user, default(System.Threading.CancellationToken));
            User newUser = await Context.LoadAsync<User>(user.Email, default(System.Threading.CancellationToken));
            return user;
        }

        public async Task<User> LogIn(String email, String password)
        {
            User userDB = await Context.LoadAsync<User>(email);
            if (userDB != null)
            {
                if (!(userDB.Password.Equals(password)))
                {
                   userDB = null;
                }
            }
            return userDB;
        }

        public async Task<List<Movie>> GetMovies()
        {
            var conditions = new List<ScanCondition>();
            List<Movie> movies= await Context.ScanAsync<Movie>(conditions).GetRemainingAsync();
            foreach(Movie movie in movies)
                movie.Cover.DownloadTo(Tmp + movie.Id + movie.Cover.GetType());
            return movies;
        }

        public async Task<Movie> GetMovie(string Id, Boolean WithVideo)
        {
            Movie movie = await Context.LoadAsync<Movie>(Id, default(System.Threading.CancellationToken));
            movie.Cover.DownloadTo(Tmp + movie.Id + movie.Cover.GetType());
            if (WithVideo)
                movie.Video.DownloadTo(Tmp + movie.Id + movie.Video.GetType());
            return movie;
        }

        public async Task<Movie> UploadMovie(String bucketName, String Title, String VideoPath, String CoverPath)
        {
            Movie movie = new Movie();
            movie.Id = System.Guid.NewGuid().ToString();
            movie.Title = Title;
            movie.Cover = S3Link.Create(Context, bucketName, movie.Id + "1", BucketRegion);
            movie.Video = S3Link.Create(Context, bucketName, movie.Id + "2", BucketRegion);

            movie.Cover.UploadFrom(CoverPath);
            movie.Video.UploadFrom(VideoPath);

            await Context.SaveAsync<Movie>(movie);
            return await GetMovie(movie.Id,false);
        }

        public async void AddComment(string email,string Id, String comment, int rate)
        {
            Movie movie = await GetMovie(Id, false);
            movie.Ratings.Add(new Rating()
            {
                Date = DateTime.Now,
                Comment = comment,
                Rate = rate,
                User = await Context.LoadAsync<User>(email)
            });

            await Context.SaveAsync<Movie>(movie);
        }

        public User GetUser(string email)
        {
            return Context.Load<User>(email);
        }


        public void CreateTable()
        {
            String tableName = "User";
            List<string> currentTables = Client.ListTables().TableNames;
            bool tablesAdded = false;
            if (!currentTables.Contains(tableName))
            {
                Client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = tableName,
                    ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 3, WriteCapacityUnits = 1 },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Email",
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition { AttributeName = "Email", AttributeType = ScalarAttributeType.S}
                    },
                });

                Client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = "Movie",
                    ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 3, WriteCapacityUnits = 1 },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition { AttributeName = "Id", AttributeType = ScalarAttributeType.S}
                    },
                });
                tablesAdded = true;
            }

            if (tablesAdded)
            {
                bool allActive;
                do
                {
                    allActive = true;
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    TableStatus tableStatus = GetTableStatus(Client, tableName);
                    if (!object.Equals(tableStatus, TableStatus.ACTIVE))
                        allActive = false;

                } while (!allActive);
            }
        }


        private static TableStatus GetTableStatus(AmazonDynamoDBClient client, string tableName)
        {
            try
            {
                var table = client.DescribeTable(new DescribeTableRequest { TableName = tableName }).Table;
                return (table == null) ? null : table.TableStatus;
            }
            catch (AmazonDynamoDBException db)
            {
                if (db.ErrorCode == "ResourceNotFoundException")
                    return string.Empty;
                throw;
            }
        }

    }
}
