﻿using System;
using System.IO;
using NUnit.Framework;
using UE4Config.Parsing;

namespace UE4Config.Tests.Parsing
{
    [TestFixture]
    class MultilineTokenTests
    {
        [TestCase(typeof(WhitespaceToken))]
        [TestCase(typeof(CommentToken))]
        public void When_ConstructedDefault(Type tokenType)
        {
            var token = System.Activator.CreateInstance(tokenType, new object[] { }) as MultilineToken;
            Assert.That(token, Is.Not.Null);
            Assert.That(token.Lines, Is.Not.Null);
            Assert.That(token.Lines, Is.Empty);
        }

        [TestCase(typeof(WhitespaceToken), new[]{""})]
        [TestCase(typeof(WhitespaceToken), new[] { " ", "\t" })]
        [TestCase(typeof(CommentToken), new[]{"; Comment"})]
        [TestCase(typeof(CommentToken), new[] { ";Multi", " ;Line", "; Comment" })]
        public void When_ConstructedWithLines(Type tokenType, string[] lines)
        {
            var token = System.Activator.CreateInstance(tokenType, new object[] { lines }) as MultilineToken;
            Assert.That(token, Is.Not.Null);
            Assert.That(token.Lines, Is.Not.Null);
            Assert.That(token.Lines, Is.EquivalentTo(lines));
        }
    }
}