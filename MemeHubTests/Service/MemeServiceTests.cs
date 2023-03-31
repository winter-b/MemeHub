using System;
using Xunit;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repositories;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace MemeHubTests.Service
{
    public class MemeServiceTests
    {
        private Mock<IMemeRepository> _mockMemeRepository;
        private Mock<IHackingService> _mockHackingService;
        private IMemeService _memeService;

        public MemeServiceTests()
        {
            _mockMemeRepository = new Mock<IMemeRepository>();
            _mockHackingService = new Mock<IHackingService>();
            _memeService = new MemeService(_mockMemeRepository.Object, _mockHackingService.Object, "key", "jwtKey");
        }

        [Fact]
        public void GetMemes_ShouldCallMemeRepositoryWithCorrectParameters()
        {
            // Arrange
            const int limit = 10;
            const int offset = 0;
            var expectedMemes = new List<Meme>();
            _mockMemeRepository.Setup(x => x.GetMemes(limit, offset)).Returns(expectedMemes);

            // Act
            var actualMemes = _memeService.GetMemes(limit, offset);

            // Assert
            Assert.Equal(expectedMemes, actualMemes);
            _mockMemeRepository.Verify(x => x.GetMemes(limit, offset));
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(10, 10)]
        [InlineData(20, 5)]

        public void GetMemes_ReturnsCorrectSubset(int offset, int limit)
        { // Arrange
            var memes = new List<Meme>();
            for (int i = 0; i < limit+offset; i++)
            {
                memes.Add(new Meme { Id = new Guid(), Caption = $"Meme {i}" });
            }
            _mockMemeRepository.Setup(x => x.GetMemes(offset, limit)).Returns(memes.Skip(offset).Take(limit).ToList());

            // Act
            var result = _memeService.GetMemes(offset, limit);

            // Assert
            Assert.Equal(memes.Skip(offset).Take(limit).ToList(), result);
        }


        [Fact]
        public void GetMeme_ShouldCallMemeRepositoryWithCorrectParameters()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedMeme = new Meme();
            _mockMemeRepository.Setup(x => x.GetMeme(id)).Returns(expectedMeme);

            // Act
            var actualMeme = _memeService.GetMeme(id);

            // Assert
            Assert.Equal(expectedMeme, actualMeme);
            _mockMemeRepository.Verify(x => x.GetMeme(id));
        }
    }
}
