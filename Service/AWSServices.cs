using System;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Threading;

namespace WebApplicationBeanstalk.Service
{
    public class AWSServices
    {
        AmazonDynamoDBClient client;
        DynamoDBContext context;
        RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
        string tmp = AppDomain.CurrentDomain.BaseDirectory;


        public UserRepository()
        {
            client = new AmazonDynamoDBClient(bucketRegion);
            context = new DynamoDBContext(client);
            CreateTable();
        }

        public User Register(String email, String name, String password)
        {
            User user = new User()
            {
                Email = email,
                Name = name,
                Password = password
            };

            context.Save<User>(user);
            return user;
        }

        public User LogIn(String email, String password)
        {
            User userDB = context.Load<User>(email);
            if (userDB != null)
            {
                if (!(userDB.Password.Equals(password)))
                {
                    userDB = null;
                }
            }

            return userDB;
        }

        public User GetUser(String email)
        {
            return context.Load<User>(email);
        }

        public void AddBook(String email, String title, String bucketName, String coverPath, String pdfPath)
        {

            String date = DateTime.Now.ToString("yyyyMMddTHHmmssfff");

            MyBook myBook = new MyBook();
            myBook.Cover = S3Link.Create(context, bucketName, email + "1" + date, bucketRegion);
            myBook.PDFFile = S3Link.Create(context, bucketName, email + "2" + date, bucketRegion);
            myBook.Title = title;
            myBook.Page = 1;
            myBook.IsLastBook = true;

            myBook.Cover.UploadFrom(coverPath);
            myBook.PDFFile.UploadFrom(pdfPath);

            User userDB = context.Load<User>(email, new DynamoDBContextConfig
            {
                ConsistentRead = true
            });

            if (userDB.Books == null)
            {
                userDB.Books = new List<MyBook>();
            }
            else
            {
                foreach (MyBook book in userDB.Books)
                {
                    book.IsLastBook = false;
                }
            }
            userDB.Books.Add(myBook);
            context.Save<User>(userDB);
        }

        public void UpdatePage(String email, String title, int lastPage)
        {
            User userDB = context.Load<User>(email, new DynamoDBContextConfig
            {
                ConsistentRead = true
            });

            foreach (MyBook myBook in userDB.Books)
            {
                if (myBook.Title.Equals(title))
                {
                    myBook.Page = lastPage;
                    break;
                }
            }
            context.Save<User>(userDB);
        }

        public MyBook ReadBook(String email, String title)
        {
            User userDB = context.Load<User>(email, new DynamoDBContextConfig
            {
                ConsistentRead = true
            });
            MyBook currentBook = null;
            foreach (MyBook myBook in userDB.Books)
            {
                if (myBook.Title.Equals(title))
                {
                    myBook.IsLastBook = true;
                    currentBook = myBook;
                }
                else
                {
                    myBook.IsLastBook = false;
                }
            }
            context.Save<User>(userDB);
            DownloadFiles(currentBook);
            return currentBook;
        }


        public void DownloadFiles(MyBook myBook)
        {
            Boolean found = false;
            int attemps = 0;
            while (!found && attemps <= 5)
            {
                try
                {
                    if (!System.IO.File.Exists(tmp + myBook.Title + ".jpg"))
                        myBook.Cover.DownloadTo(tmp + myBook.Title + ".jpg");
                    if (!System.IO.File.Exists(tmp + myBook.Title + ".pdf"))
                        myBook.PDFFile.DownloadTo(tmp + myBook.Title + ".pdf");

                    if (System.IO.File.Exists(tmp + myBook.Title + ".jpg")
                        && System.IO.File.Exists(tmp + myBook.Title + ".pdf"))
                    {
                        found = true;
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed attempt (" + attemps + ") to retrieve the book (" + myBook.Title + ")");
                    attemps++;
                }
            }


        }


        public List<MyBook> GetAllBooks(String email)
        {
            User userDB = context.Load<User>(email);
            List<MyBook> myBooks = new List<MyBook>();
            foreach (var book in userDB.Books)
            {
                myBooks.Add(book);
            }
            return myBooks;
        }

        public MyBook GetLastBook(String email)
        {
            User userDB = context.Load<User>(email);
            MyBook lastBook = null;
            if (userDB.Books != null)
            {
                foreach (MyBook myBook in userDB.Books)
                {
                    if (myBook.IsLastBook)
                    {
                        lastBook = myBook;
                        DownloadFiles(lastBook);
                    }
                }
            }

            return lastBook;
        }


        #region Table operations (Create and Get Status)
        public void CreateTable()
        {
            String tableName = "User";
            List<string> currentTables = client.ListTables().TableNames;
            bool tablesAdded = false;
            if (!currentTables.Contains(tableName))
            {
                client.CreateTableAsync(new CreateTableRequest
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
                tablesAdded = true;
            }

            if (tablesAdded)
            {
                bool allActive;
                do
                {
                    allActive = true;
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    TableStatus tableStatus = GetTableStatus(client, tableName);
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
        #endregion
    }
}
