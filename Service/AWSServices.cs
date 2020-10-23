using System;
using System.Collections.Generic;
using System.Threading;
using WebApplicationBeanstalk.Models;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using System.Collections;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace WebApplicationBeanstalk.Service
{
    public class AWSServices
    {
        string Tmp = AppDomain.CurrentDomain.BaseDirectory;
        RegionEndpoint Region = RegionEndpoint.CACentral1;

        IAmazonDynamoDB dynamoDBClient { get; set; }
        IAmazonS3 s3Client { get; set; }

        public AWSServices(IAmazonDynamoDB dynamoDBClient, IAmazonS3 s3Client)
        {
            this.dynamoDBClient = dynamoDBClient;
            this.s3Client = s3Client;
            CreateTable();
        }
        public async Task<User> Register(User user)
        {
            DynamoDBContext Context = new DynamoDBContext(dynamoDBClient);
            await Context.SaveAsync(user, default(System.Threading.CancellationToken));
            User newUser = await Context.LoadAsync<User>(user.Email, default(System.Threading.CancellationToken));
            return user;
        }


        public List<String> GetBuckets()
        {
            List<String> buckets = new List<string>();
            ListBucketsResponse response = s3Client.ListBucketsAsync().Result;
            foreach (S3Bucket bucket in response.Buckets)
            {
                buckets.Add(bucket.BucketName);
            }
            return buckets;
        }
        
        public async Task<User> LogIn(String email, String password)
        {
            DynamoDBContext Context = new DynamoDBContext(dynamoDBClient);
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
            DynamoDBContext Context = new DynamoDBContext(dynamoDBClient);

            var conditions = new List<ScanCondition>();
            List<Movie> movies= await Context.ScanAsync<Movie>(conditions).GetRemainingAsync();
            foreach(Movie movie in movies)
            {
                Task task1 = s3Client.DownloadToFilePathAsync(
                    movie.Cover.BucketName,
                    movie.Cover.Key, Tmp ,
                    new Dictionary<string, object>(),
                    default(System.Threading.CancellationToken));

            }
                
            return movies;
        }

        public Task<Movie> GetMovie(string Id)
        {
            DynamoDBContext Context = new DynamoDBContext(dynamoDBClient);

            return Context.LoadAsync<Movie>(Id, default(System.Threading.CancellationToken));
            
        }

        public async Task<Movie> UploadMovie(String bucketName, String Title, System.IO.Stream VideoPath, System.IO.Stream CoverPath)
        {
            DynamoDBContext Context = new DynamoDBContext(dynamoDBClient);

            Movie movie = new Movie();
            movie.Id = System.Guid.NewGuid().ToString();
            movie.Title = Title;
            movie.Cover = S3Link.Create(Context, bucketName, movie.Id + "1.jpg", Region);
            movie.Video = S3Link.Create(Context, bucketName, movie.Id + "2.avi", Region);

            var fileTransferUtility = new TransferUtility(s3Client);
            var cover = new TransferUtilityUploadRequest()
            {
                CannedACL = S3CannedACL.PublicRead,
                BucketName = bucketName,
                Key = movie.Id + "1.jpg",
                InputStream = CoverPath
            };

            var video = new TransferUtilityUploadRequest()
            {
                CannedACL = S3CannedACL.PublicRead,
                BucketName = bucketName,
                Key = movie.Id + "2.avi",
                InputStream = VideoPath
            };
            await fileTransferUtility.UploadAsync(cover);
            await fileTransferUtility.UploadAsync(video);

            await Context.SaveAsync<Movie>(movie);
            return await GetMovie(movie.Id);
        }

        //public async  void S3Bucket GetS3Bucket(string bucket)
        //{
        //    return  s3Client.ListBucketsAsync(). Buckets.Where(b => b.BucketName == bucket).Single();
        //}

        public async void AddComment(string email,string Id, String comment, int rate)
        {
            DynamoDBContext Context = new DynamoDBContext(dynamoDBClient);

            Movie movie = await GetMovie(Id);
            movie.Ratings.Add(new Rating()
            {
                Date = DateTime.Now,
                Comment = comment,
                Rate = rate,
                User = await Context.LoadAsync<User>(email)
            });

            await Context.SaveAsync<Movie>(movie);
        }

        public async Task<User> GetUser(string email)
        {
            DynamoDBContext Context = new DynamoDBContext(dynamoDBClient);

            return await Context.LoadAsync<User>(email);
        }


        public void CreateTable()
        {
            String tableName = "User";
            Task<ListTablesResponse> table = dynamoDBClient.ListTablesAsync();
            List<string> currentTables = table.Result.TableNames;
            bool tablesAdded = false;
            if (!currentTables.Contains(tableName))
            {
                dynamoDBClient.CreateTableAsync(new CreateTableRequest
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

                dynamoDBClient.CreateTableAsync(new CreateTableRequest
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

                    TableStatus tableStatus = GetTableStatus(tableName);
                    if (!object.Equals(tableStatus, TableStatus.ACTIVE))
                        allActive = false;

                } while (!allActive);
            }
        }


        private TableStatus GetTableStatus(string tableName)
        {
            try
            {
                Task<DescribeTableResponse> tableResp = dynamoDBClient.DescribeTableAsync(new DescribeTableRequest { TableName = tableName });
                TableDescription table = tableResp.Result.Table;
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
