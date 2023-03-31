using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repositories;
using Xunit;

namespace WebApi.Tests.Repositories
{
    public class MemeRepositoryTests
    {
        private readonly Mock<IMongoClient> _mockMongoClient;
        private readonly Mock<IMongoCollection<Meme>> _mockMemeCollection;
        private readonly IMemeRepository _repository;

        public MemeRepositoryTests()
        {
            _mockMongoClient = new Mock<IMongoClient>();
            _mockMemeCollection = new Mock<IMongoCollection<Meme>>();
            //_mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(Mock.Of<IMongoDatabase>());
            _mockMongoClient.Setup(x => x.GetDatabase("memeHub", It.IsAny<MongoDatabaseSettings>()).GetCollection<Meme>("memes", It.IsAny<MongoCollectionSettings>())).Returns(_mockMemeCollection.Object);

            _repository = new MemeRepository(_mockMongoClient.Object);
        }


        [Fact]
        public void GetMeme_ReturnsCorrectMeme()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedMeme = new Meme { Id = id, Caption = "Test Meme" };
            var mockCursor = new Mock<IAsyncCursor<Meme>>();
            mockCursor.Setup(x => x.Current).Returns(new[] { expectedMeme });
            mockCursor.Setup(x => x.MoveNext(default)).Returns(true);
            mockCursor.Setup(x => x.MoveNextAsync(default)).ReturnsAsync(true);
            _mockMemeCollection.Setup(x => x.FindSync(It.IsAny<FilterDefinition<Meme>>(), It.IsAny<FindOptions<Meme>>(), default)).Returns(mockCursor.Object);


            // Act
            var actualMeme = _repository.GetMeme(id);

            // Assert
            Assert.Equal(expectedMeme, actualMeme);
            //_mockMemeCollection.Verify(x => x.FindSync(It.IsAny<FilterDefinition<Meme>>(), It.IsAny<FindOptions<Meme, Meme>>(), default), Times.Once);
        }
    }
}

