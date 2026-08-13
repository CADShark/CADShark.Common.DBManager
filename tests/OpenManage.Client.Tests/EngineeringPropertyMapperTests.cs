using OpenManage.Client.Mapping;
using System;
using System.Collections.Generic;
using Xunit;

namespace OpenManage.Client.Tests
{
    public sealed class EngineeringPropertyMapperTests
    {
        [Fact]
        public void Map_MapsConfiguredPropertiesAndRelativePath()
        {
            var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Обозначение"] = "PRT-001",
                ["Наименование"] = "Корпус"
            };
            var mappings = new[]
            {
                new PropertyAttributeMapping
                {
                    PropertyName = "Обозначение",
                    AttributeId = 9
                },
                new PropertyAttributeMapping
                {
                    PropertyName = "Наименование",
                    AttributeId = 10
                }
            };

            var result = new EngineeringPropertyMapper().Map(
                properties,
                mappings,
                @"Project\Part1.sldprt");

            Assert.Collection(
                result,
                item =>
                {
                    Assert.Equal(9, item.AttributeId);
                    Assert.Equal("PRT-001", item.Value);
                },
                item =>
                {
                    Assert.Equal(10, item.AttributeId);
                    Assert.Equal("Корпус", item.Value);
                },
                item =>
                {
                    Assert.Equal(1038, item.AttributeId);
                    Assert.Equal(@"Project\Part1.sldprt", item.Value);
                });
        }

        [Fact]
        public void Map_SkipsPropertyThatDoesNotExist()
        {
            var result = new EngineeringPropertyMapper().Map(
                new Dictionary<string, string>(),
                new[]
                {
                    new PropertyAttributeMapping
                    {
                        PropertyName = "Обозначение",
                        AttributeId = 9
                    }
                },
                "Part1.sldprt");

            Assert.Single(result);
            Assert.Equal(1038, result[0].AttributeId);
        }

        [Fact]
        public void Map_RejectsDuplicateAttributeIds()
        {
            var mapper = new EngineeringPropertyMapper();
            var mappings = new[]
            {
                new PropertyAttributeMapping
                {
                    PropertyName = "Обозначение",
                    AttributeId = 9
                },
                new PropertyAttributeMapping
                {
                    PropertyName = "Наименование",
                    AttributeId = 9
                }
            };

            Assert.Throws<ArgumentException>(
                () => mapper.Map(
                    new Dictionary<string, string>(),
                    mappings,
                    "Part1.sldprt"));
        }

        [Fact]
        public void Map_RejectsManualRelativePathMapping()
        {
            var mapper = new EngineeringPropertyMapper();

            Assert.Throws<ArgumentException>(
                () => mapper.Map(
                    new Dictionary<string, string>(),
                    new[]
                    {
                        new PropertyAttributeMapping
                        {
                            PropertyName = "Path",
                            AttributeId = 1038
                        }
                    },
                    "Part1.sldprt"));
        }
    }
}
