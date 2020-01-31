﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UE4Config.Parsing;

namespace UE4Config.Tests.Parsing
{
    [TestFixture]
    class ConfigIniSectionTests
    {

        [Test]
        public void When_ConstructedDefault()
        {
            ConfigIniSection section = null;
            Assert.That(() => { section = new ConfigIniSection(); }, Throws.Nothing);
            Assert.That(section.Name, Is.Null);
            Assert.That(section.Tokens, Is.Empty);
        }

        [Test]
        public void When_ConstructedWithName()
        {
            string name = "/Script/Engine.PlayerInput";
            ConfigIniSection section = null;
            Assert.That(() => { section = new ConfigIniSection(name); }, Throws.Nothing);
            Assert.That(section.Name, Is.EqualTo(name));
            Assert.That(section.Tokens, Is.Empty);
        }

        [Test]
        public void When_ConstructedWithTokens()
        {
            ConfigIniSection section = null;
            var token1 = new TextToken();
            var token2 = new TextToken();
            Assert.That(() => { section = new ConfigIniSection(new[] { token1, token2 }); }, Throws.Nothing);
            Assert.That(section.Name, Is.Null);
            Assert.That(section.Tokens, Is.EquivalentTo(new[] { token1, token2 }));
        }

        [Test]
        public void When_ConstructedWithNullTokens()
        {
            ConfigIniSection section = null;
            Assert.That(() => { section = new ConfigIniSection((IEnumerable<IniToken>)null); }, Throws.Nothing);
            Assert.That(section.Name, Is.Null);
            Assert.That(section.Tokens, Is.Empty);
        }

        [Test]
        public void When_ConstructedWithNameAndTokens()
        {
            string name = "/Script/Engine.PlayerInput";
            ConfigIniSection section = null;
            var token1 = new TextToken();
            var token2 = new TextToken();
            Assert.That(() => { section = new ConfigIniSection(name, new[] { token1, token2 }); }, Throws.Nothing);
            Assert.That(section.Name, Is.EqualTo(name));
            Assert.That(section.Tokens, Is.EquivalentTo(new[] { token1, token2 }));
        }

        [Test]
        public void When_ConstructedWithNameAndNullTokens()
        {
            string name = "/Script/Engine.PlayerInput";
            ConfigIniSection section = null;
            Assert.That(() => { section = new ConfigIniSection(name, null); }, Throws.Nothing);
            Assert.That(section.Name, Is.EqualTo(name));
            Assert.That(section.Tokens, Is.Empty);
        }

        [TestFixture]
        class Write
        {
            public static IEnumerable Cases_WriteHeader
            {
                get
                {
                    var expectedLineEnding = Environment.NewLine;

                    yield return new TestCaseData(new object[] { new ConfigIniSection() { }, $"{expectedLineEnding}" }).SetName($"Unnamed Section, Unspecified LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection() { LineWastePrefix = " " }, $" {expectedLineEnding}" }).SetName($"Unnamed Section with LineWastePrefix, Unspecified LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection() { LineWasteSuffix = " " }, $" {expectedLineEnding}" }).SetName($"Unnamed Section with LineWasteSuffix, Unspecified LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection() { LineWastePrefix = " ", LineWasteSuffix = " " }, $"  {expectedLineEnding}" }).SetName($"Unnamed Section with LineWaste, Unspecified LineEnding");

                    string sectionName = "/Script/Engine.PlayerInput";

                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { }, $"[{sectionName}]{expectedLineEnding}" }).SetName($"Section, Unspecified LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineWastePrefix = " " }, $" [{sectionName}]{expectedLineEnding}" }).SetName($"Section with LineWastePrefix, Unspecified LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineWasteSuffix = " " }, $"[{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWasteSuffix, Unspecified LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineWastePrefix = " ", LineWasteSuffix = " " }, $" [{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWaste, Unspecified LineEnding");

                    var lineEnding = LineEnding.Unknown;

                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding }, $"[{sectionName}]{expectedLineEnding}" }).SetName($"Section, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " " }, $" [{sectionName}]{expectedLineEnding}" }).SetName($"Section with LineWastePrefix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWasteSuffix = " " }, $"[{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWasteSuffix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " ", LineWasteSuffix = " " }, $" [{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWaste, {lineEnding} LineEnding");

                    lineEnding = LineEnding.None;
                    expectedLineEnding = "";

                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding }, $"[{sectionName}]{expectedLineEnding}" }).SetName($"Section, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " " }, $" [{sectionName}]{expectedLineEnding}" }).SetName($"Section with LineWastePrefix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWasteSuffix = " " }, $"[{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWasteSuffix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " ", LineWasteSuffix = " " }, $" [{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWaste, {lineEnding} LineEnding");

                    lineEnding = LineEnding.Unix;
                    expectedLineEnding = "\n";

                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding }, $"[{sectionName}]{expectedLineEnding}" }).SetName($"Section, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " " }, $" [{sectionName}]{expectedLineEnding}" }).SetName($"Section with LineWastePrefix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWasteSuffix = " " }, $"[{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWasteSuffix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " ", LineWasteSuffix = " " }, $" [{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWaste, {lineEnding} LineEnding");

                    lineEnding = LineEnding.Windows;
                    expectedLineEnding = "\r\n";

                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding }, $"[{sectionName}]{expectedLineEnding}" }).SetName($"Section, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " " }, $" [{sectionName}]{expectedLineEnding}" }).SetName($"Section with LineWastePrefix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWasteSuffix = " " }, $"[{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWasteSuffix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " ", LineWasteSuffix = " " }, $" [{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWaste, {lineEnding} LineEnding");

                    lineEnding = LineEnding.Mac;
                    expectedLineEnding = "\r";

                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding }, $"[{sectionName}]{expectedLineEnding}" }).SetName($"Section, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " " }, $" [{sectionName}]{expectedLineEnding}" }).SetName($"Section with LineWastePrefix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWasteSuffix = " " }, $"[{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWasteSuffix, {lineEnding} LineEnding");
                    yield return new TestCaseData(new object[] { new ConfigIniSection(sectionName) { LineEnding = lineEnding, LineWastePrefix = " ", LineWasteSuffix = " " }, $" [{sectionName}] {expectedLineEnding}" }).SetName($"Section with LineWaste, {lineEnding} LineEnding");

                }
            }

            [TestCaseSource(nameof(Cases_WriteHeader))]
            public void WriteHeader(ConfigIniSection section, string expectedText)
            {
                var writer = new StringWriter();
                section.WriteHeader(writer);
                Assert.That(writer.ToString(), Is.EqualTo(expectedText));
            }
        }


        [TestFixture]
        class MergeConsecutiveTokens
        {
            [Test]
            public void When_HasSingleWhitespaceTokens()
            {
                var section = new ConfigIniSection();
                var token1 = new WhitespaceToken(new[] { "", "\t", "  " });
                section.Tokens.Add(token1);
                section.MergeConsecutiveTokens();

                Assert.That(section.Tokens, Has.Count.EqualTo(1));
                Assert.That(section.Tokens[0], Is.SameAs(token1));
                Assert.That(token1.Lines, Is.EquivalentTo(new[] { "", "\t", "  " }));
            }

            [Test]
            public void When_Has2ConsecutiveWhitespaceTokens()
            {
                var section = new ConfigIniSection();
                var token1 = new WhitespaceToken(new[] { "", "\t", "  " });
                var token2 = new WhitespaceToken(new[] { " ", "\t", "" });
                section.Tokens.Add(token1);
                section.Tokens.Add(token2);
                section.MergeConsecutiveTokens();
                
                Assert.That(section.Tokens, Has.Count.EqualTo(1));
                Assert.That(section.Tokens[0], Is.SameAs(token1));
                Assert.That(token1.Lines, Is.EquivalentTo(new[] { "", "\t", "  ", " ", "\t", "" }));
            }

            [Test]
            public void When_Has3ConsecutiveWhitespaceTokens()
            {
                var section = new ConfigIniSection();
                var token1 = new WhitespaceToken(new[] { "", "\t", "  " });
                var token2 = new WhitespaceToken(new[] { " ", "\t", "" });
                var token3 = new WhitespaceToken(new[] { " \t\t" });
                section.Tokens.Add(token1);
                section.Tokens.Add(token2);
                section.Tokens.Add(token3);
                section.MergeConsecutiveTokens();

                Assert.That(section.Tokens, Has.Count.EqualTo(1));
                Assert.That(section.Tokens[0], Is.SameAs(token1));
                Assert.That(token1.Lines, Is.EquivalentTo(new[] { "", "\t", "  ", " ", "\t", "", " \t\t" }));
            }

            [Test]
            public void When_HasSingleCommentToken()
            {
                var section = new ConfigIniSection();
                var token1 = new CommentToken(new[] { "; Hey", ";Whats", " ;Up?" });
                section.Tokens.Add(token1);
                section.MergeConsecutiveTokens();

                Assert.That(section.Tokens, Has.Count.EqualTo(1));
                Assert.That(section.Tokens[0], Is.SameAs(token1));
                Assert.That(token1.Lines, Is.EquivalentTo(new[] { "; Hey", ";Whats", " ;Up?" }));
            }

            [Test]
            public void When_Has2ConsecutiveCommentTokens()
            {
                var section = new ConfigIniSection();
                var token1 = new CommentToken(new[] { "; Hey", ";Whats", " ;Up?" });
                var token2 = new CommentToken(new[] { ";Foo", ";Bar" });
                section.Tokens.Add(token1);
                section.Tokens.Add(token2);
                section.MergeConsecutiveTokens();

                Assert.That(section.Tokens, Has.Count.EqualTo(1));
                Assert.That(section.Tokens[0], Is.SameAs(token1));
                Assert.That(token1.Lines, Is.EquivalentTo(new[] { "; Hey", ";Whats", " ;Up?", ";Foo", ";Bar" }));
            }

            [Test]
            public void When_Has3ConsecutiveCommentTokens()
            {
                var section = new ConfigIniSection();
                var token1 = new CommentToken(new[] { "; Hey", ";Whats", " ;Up?" });
                var token2 = new CommentToken(new[] { ";Foo", ";Bar" });
                var token3 = new CommentToken(new[] { ";Baz" });
                section.Tokens.Add(token1);
                section.Tokens.Add(token2);
                section.Tokens.Add(token3);
                section.MergeConsecutiveTokens();

                Assert.That(section.Tokens, Has.Count.EqualTo(1));
                Assert.That(section.Tokens[0], Is.SameAs(token1));
                Assert.That(token1.Lines, Is.EquivalentTo(new[] { "; Hey", ";Whats", " ;Up?", ";Foo", ";Bar", ";Baz" }));
            }

            [Test]
            public void When_Has3NonConsecutiveTokens()
            {
                var section = new ConfigIniSection();
                var token1 = new CommentToken(new[] { "; Hey", ";Whats", " ;Up?" });
                var token2 = new WhitespaceToken(new[] { " ", "\t", "" });
                var token3 = new CommentToken(new[] { ";Baz" });
                section.Tokens.Add(token1);
                section.Tokens.Add(token2);
                section.Tokens.Add(token3);
                section.MergeConsecutiveTokens();

                Assert.That(section.Tokens, Has.Count.EqualTo(3));
                Assert.That(section.Tokens[0], Is.SameAs(token1));
                Assert.That(section.Tokens[1], Is.SameAs(token2));
                Assert.That(section.Tokens[2], Is.SameAs(token3));
                Assert.That(token1.Lines, Is.EquivalentTo(new[] { "; Hey", ";Whats", " ;Up?" }));
                Assert.That(token2.Lines, Is.EquivalentTo(new[] { " ", "\t", "" }));
                Assert.That(token3.Lines, Is.EquivalentTo(new[] { ";Baz" }));
            }

            [Test]
            public void When_Has4TokensWith2ConsecutiveInBetween()
            {
                var section = new ConfigIniSection();
                var token1 = new CommentToken(new[] { "; Hey", ";Whats", " ;Up?" });
                var token2 = new WhitespaceToken(new[] { " ", "\t", "" });
                var token3 = new WhitespaceToken(new[] { " \t\t" });
                var token4 = new CommentToken(new[] { ";Baz" });
                section.Tokens.Add(token1);
                section.Tokens.Add(token2);
                section.Tokens.Add(token3);
                section.Tokens.Add(token4);
                section.MergeConsecutiveTokens();

                Assert.That(section.Tokens, Has.Count.EqualTo(3));
                Assert.That(section.Tokens[0], Is.SameAs(token1));
                Assert.That(section.Tokens[1], Is.SameAs(token2));
                Assert.That(section.Tokens[2], Is.SameAs(token4));
                Assert.That(token1.Lines, Is.EquivalentTo(new[] { "; Hey", ";Whats", " ;Up?" }));
                Assert.That(token2.Lines, Is.EquivalentTo(new[] { " ", "\t", "", " \t\t" }));
                Assert.That(token4.Lines, Is.EquivalentTo(new[] { ";Baz" }));
            }
        }
    }
}
