﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Tests.Compiler.Syntax
{
	public static class LexerTests
	{
		[TestCaseSource(nameof(LexerTests.GetTokensData))]
		public static void LexerLexesToken((SyntaxKind kind, string text) value)
		{
			var tokens = SyntaxTree.ParseTokens(value.text).ToArray();

			Assert.Multiple(() =>
			{
				Assert.That(tokens.Length, Is.EqualTo(1), nameof(tokens.Length));
				var token = tokens[0];
				Assert.That(token.Kind, Is.EqualTo(value.kind), nameof(token.Kind));
				Assert.That(token.Text, Is.EqualTo(value.text), nameof(token.Text));
			});
		}

		[TestCaseSource(nameof(LexerTests.GetTokenPairsData))]
		public static void LexerLexesTokenPairs((SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text) value)
		{
			var text = $"{value.t1Text}{value.t2Text}";
			var tokens = SyntaxTree.ParseTokens(text).ToArray();

			Assert.Multiple(() =>
			{
				Assert.That(tokens.Length, Is.EqualTo(2), nameof(tokens.Length));
				var token1 = tokens[0];
				Assert.That(token1.Kind, Is.EqualTo(value.t1Kind), $"1 - {nameof(token1.Kind)}");
				Assert.That(token1.Text, Is.EqualTo(value.t1Text), $"1 - {nameof(token1.Text)}");
				var token2 = tokens[1];
				Assert.That(token2.Kind, Is.EqualTo(value.t2Kind), $"2 - {nameof(token2.Kind)}");
				Assert.That(token2.Text, Is.EqualTo(value.t2Text), $"2 - {nameof(token2.Text)}");
			});
		}

		[TestCaseSource(nameof(LexerTests.GetTokenPairsWithSeparatorsData))]
		public static void LexerLexesTokenPairsWithSeparators((SyntaxKind t1Kind, string t1Text, SyntaxKind separatorKind, string separatorText, SyntaxKind t2Kind, string t2Text) value)
		{
			var text = $"{value.t1Text}{value.separatorText}{value.t2Text}";
			var tokens = SyntaxTree.ParseTokens(text).ToArray();

			Assert.Multiple(() =>
			{
				Assert.That(tokens.Length, Is.EqualTo(3), nameof(tokens.Length));
				var token1 = tokens[0];
				Assert.That(token1.Kind, Is.EqualTo(value.t1Kind), $"1 - {nameof(token1.Kind)}");
				Assert.That(token1.Text, Is.EqualTo(value.t1Text), $"1 - {nameof(token1.Text)}");
				var token2 = tokens[1];
				Assert.That(token2.Kind, Is.EqualTo(value.separatorKind), $"2 - {nameof(token2.Kind)}");
				Assert.That(token2.Text, Is.EqualTo(value.separatorText), $"2 - {nameof(token2.Text)}");
				var token3 = tokens[2];
				Assert.That(token3.Kind, Is.EqualTo(value.t2Kind), $"3 - {nameof(token3.Kind)}");
				Assert.That(token3.Text, Is.EqualTo(value.t2Text), $"3 - {nameof(token3.Text)}");
			});
		}

		private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators() =>
			new[]
			{
				(SyntaxKind.WhitespaceToken, " "),
				(SyntaxKind.WhitespaceToken, "  "),
				(SyntaxKind.WhitespaceToken, "\t"),
				(SyntaxKind.WhitespaceToken, "\r"),
				(SyntaxKind.WhitespaceToken, "\n"),
				(SyntaxKind.WhitespaceToken, "\r\n"),
			};

		private static bool RequiresSeparator(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)
		{
			var t1IsKeyword = t1Kind.ToString().EndsWith("Keyword");
			var t2IsKeyword = t2Kind.ToString().EndsWith("Keyword");

			return (t1Kind == SyntaxKind.IdentifierToken && t2Kind == SyntaxKind.IdentifierToken) ||
				(t1Kind == SyntaxKind.NumberToken && t2Kind == SyntaxKind.NumberToken) ||
				(t1Kind == SyntaxKind.BangToken && t2Text == "=") ||
				(t1Kind == SyntaxKind.BangToken && t2Kind == SyntaxKind.EqualsEqualsToken) ||
				(t1Text == "=" && t2Text == "=") ||
				(t1IsKeyword && t2IsKeyword) ||
				(t1IsKeyword && t2Kind == SyntaxKind.IdentifierToken) ||
				(t2IsKeyword && t1Kind == SyntaxKind.IdentifierToken);
		}

		private static IEnumerable<(SyntaxKind kind, string text)> GetTokensData()
		{
			foreach (var (tokenKind, tokenText) in LexerTests.GetTokens().Concat(LexerTests.GetSeparators()))
			{
				yield return (tokenKind, tokenText);
			}
		}

		private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairsData()
		{
			foreach (var (t1Kind, t1Text, t2Kind, t2Text) in LexerTests.GetTokenPairs())
			{
				yield return (t1Kind, t1Text, t2Kind, t2Text);
			}
		}

		private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind separatorKind, string separatorText, SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeparatorsData()
		{
			foreach (var (t1Kind, t1Text, separatorKind, separatorText, t2Kind, t2Text) in LexerTests.GetTokenPairsWithSeparators())
			{
				yield return (t1Kind, t1Text, separatorKind, separatorText, t2Kind, t2Text);
			}
		}

		private static IEnumerable<(SyntaxKind kind, string text)> GetTokens() =>
			new[]
			{
				(SyntaxKind.FalseKeyword, "false"),
				(SyntaxKind.TrueKeyword, "true"),
				(SyntaxKind.OpenParenthesisToken, "("),
				(SyntaxKind.BangEqualsToken, "!="),
				(SyntaxKind.EqualsEqualsToken, "=="),
				(SyntaxKind.PipePipeToken, "||"),
				(SyntaxKind.AmpersandAmpersandToken, "&&"),
				(SyntaxKind.BangToken, "!"),
				(SyntaxKind.SlashToken, "/"),
				(SyntaxKind.StarToken, "*"),
				(SyntaxKind.MinusToken, "-"),
				(SyntaxKind.PlusToken, "+"),

				(SyntaxKind.IdentifierToken, "a"),
				(SyntaxKind.IdentifierToken, "abc"),
				(SyntaxKind.NumberToken, "1"),
				(SyntaxKind.NumberToken, "123"),
			};

		private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
		{
			foreach (var (t1Kind, t1Text) in LexerTests.GetTokens())
			{
				foreach (var (t2Kind, t2Text) in LexerTests.GetTokens())
				{
					if (!LexerTests.RequiresSeparator(t1Kind, t1Text, t2Kind, t2Text))
					{
						yield return (t1Kind, t1Text, t2Kind, t2Text);
					}
				}
			}
		}

		private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind separatorKind, string separatorText, SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeparators()
		{
			foreach (var (t1Kind, t1Text) in LexerTests.GetTokens())
			{
				foreach (var (t2Kind, t2Text) in LexerTests.GetTokens())
				{
					if (LexerTests.RequiresSeparator(t1Kind, t1Text, t2Kind, t2Text))
					{
						foreach (var (separatorKind, separatorText) in LexerTests.GetSeparators())
						{
							yield return (t1Kind, t1Text, separatorKind, separatorText, t2Kind, t2Text);
						}
					}
				}
			}
		}
	}
}