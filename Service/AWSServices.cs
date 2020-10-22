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
        RegionEndpoint BucketRegion = RegionEndpoint.CNNorth1;

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
            return await Context.ScanAsync<Movie>(conditions).GetRemainingAsync(); ;
        }

        public async Task<Movie> GetMovie(string Id)
        {
            return await Context.LoadAsync<Movie>(Id, default(System.Threading.CancellationToken));
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
            return await GetMovie(movie.Id);
        }


        //public void AddBook(String email, String title, String bucketName, String coverPath, String pdfPath)
        //{

        //    String date = DateTime.Now.ToString("yyyyMMddTHHmmssfff");

        //    Movie movie = new Movie();
        //    movie.Cover = S3Link.Create(Context, bucketName, email + "1" + date, BucketRegion);
        //    movie.PDFFile = S3Link.Create(Context, bucketName, email + "2" + date, BucketRegion);
        //    movie.Title = title;
        //    movie.Page = 1;
        //    movie.IsLastBook = true;

        //    movie.Cover.UploadFrom(coverPath);
        //    movie.PDFFile.UploadFrom(pdfPath);

        //    User userDB = Context.Load<User>(email, new DynamoDBContextConfig
        //    {
        //        ConsistentRead = true
        //    });

        //    if (userDB.Books == null)
        //    {
        //        userDB.Books = new List<Movie>();
        //    }
        //    else
        //    {
        //        foreach (Movie book in userDB.Books)
        //        {
        //            book.IsLastBook = false;
        //        }
        //    }
        //    userDB.Books.Add(movie);
        //    Context.Save<User>(userDB);
        //}

        //public void UpdatePage(String email, String title, int lastPage)
        //{
        //    User userDB = Context.Load<User>(email, new DynamoDBContextConfig
        //    {
        //        ConsistentRead = true
        //    });

        //    foreach (Movie movie in userDB.Books)
        //    {
        //        if (movie.Title.Equals(title))
        //        {
        //            movie.Page = lastPage;
        //            break;
        //        }
        //    }
        //    Context.Save<User>(userDB);
        //}

        //public Movie ReadBook(String email, String title)
        //{
        //    User userDB = Context.Load<User>(email, new DynamoDBContextConfig
        //    {
        //        ConsistentRead = true
        //    });
        //    Movie currentBook = null;
        //    foreach (Movie movie in userDB.Books)
        //    {
        //        if (movie.Title.Equals(title))
        //        {
        //            movie.IsLastBook = true;
        //            currentBook = movie;
        //        }
        //        else
        //        {
        //            movie.IsLastBook = false;
        //        }
        //    }
        //    Context.Save<User>(userDB);
        //    DownloadFiles(currentBook);
        //    return currentBook;
        //}


        //public void DownloadFiles(Movie movie)
        //{
        //    Boolean found = false;
        //    int attemps = 0;
        //    while (!found && attemps <= 5)
        //    {
        //        try
        //        {
        //            if (!System.IO.File.Exists(tmp + movie.Title + ".jpg"))
        //                movie.Cover.DownloadTo(tmp + movie.Title + ".jpg");
        //            if (!System.IO.File.Exists(tmp + movie.Title + ".pdf"))
        //                movie.PDFFile.DownloadTo(tmp + movie.Title + ".pdf");

        //            if (System.IO.File.Exists(tmp + movie.Title + ".jpg")
        //                && System.IO.File.Exists(tmp + movie.Title + ".pdf"))
        //            {
        //                found = true;
        //                break;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("Failed attempt (" + attemps + ") to retrieve the book (" + movie.Title + ")");
        //            attemps++;
        //        }
        //    }


        //}


        //public List<Movie> GetAllBooks(String email)
        //{
        //    User userDB = Context.Load<User>(email);
        //    List<Movie> movies = new List<Movie>();
        //    foreach (var book in userDB.Books)
        //    {
        //        movies.Add(book);
        //    }
        //    return movies;
        //}

        //public Movie GetLastBook(String email)
        //{
        //    User userDB = Context.Load<User>(email);
        //    Movie lastBook = null;
        //    if (userDB.Books != null)
        //    {
        //        foreach (Movie movie in userDB.Books)
        //        {
        //            if (movie.IsLastBook)
        //            {
        //                lastBook = movie;
        //                DownloadFiles(lastBook);
        //            }
        //        }
        //    }

        //    return lastBook;
        //}


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
