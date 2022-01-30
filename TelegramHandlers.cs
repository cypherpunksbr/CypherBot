using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CypherBot
{
	public class TelegramHandlers
	{
		public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var ErrorMessage = exception switch
			{
				ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};

			Console.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}

		public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var handler = update.Type switch
			{
				// UpdateType.Unknown:
				// UpdateType.ChannelPost:
				// UpdateType.EditedChannelPost:
				// UpdateType.ShippingQuery:
				// UpdateType.PreCheckoutQuery:
				// UpdateType.Poll:
				UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
				UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
				UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
				UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
				UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),
				_ => UnknownUpdateHandlerAsync(botClient, update)
			};

			try
			{
				await handler;
			}
			catch (Exception exception)
			{
				await HandleErrorAsync(botClient, exception, cancellationToken);
			}
		}

		private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
		{
			if (message.Type == MessageType.ChatMembersAdded)
			{
				new Task(() =>
					{
					Thread.Sleep(10000);

					foreach (User user in message.NewChatMembers)
					{
						if (botClient.GetChatMemberAsync(message.Chat.Id, user.Id).Result.Status == ChatMemberStatus.Left) { botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId); Console.WriteLine("usuário entrou e saiu. Mensagem apagada"); }
					}
				}).Start();
			}

			if (message.Type != MessageType.Text) { return; }

			Task<Message> action;

			action = message.Text!.Split(' ', '_')[0] switch
			{
				"/help" => Usage(botClient, message),
				"/start" => Usage(botClient, message),
				"/post" => Post(botClient, message),
				"/offtopic" => Offtopic(botClient, message),
				"/off" => Offtopic(botClient, message),

				"/help@cypherpunksbrbot" => Usage(botClient, message),
				"/start@cypherpunksbrbot" => Usage(botClient, message),
				"/post@cypherpunksbrbot" => Post(botClient, message),
				"/offtopic@cypherpunksbrbot" => Offtopic(botClient, message),
				"/off@cypherpunksbrbot" => Offtopic(botClient, message),
				_ => null
			};

			if (action == null)
			{
				action = message.Text!.ToLowerInvariant().Normalize() switch
				{
					string a when a.StartsWith("/") => BarraComandoInutil(botClient, message),
					_ => message.Chat.Type == ChatType.Private ? Usage(botClient, message) : null
				};
			}

			if (action == null) { return; }

			Console.WriteLine($"Telegram Bot: {message.Type}" + (message.Type == MessageType.Text ? ": " + message.Text : null));

			Message sentMessage = await action;
			Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

			static async Task<Message> Offtopic(ITelegramBotClient botClient, Message message)
			{
				StringBuilder sb = new();

				sb.AppendLine(Resources.Messages.offtopic_request);

				Message sentMessage = await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: sb.ToString(), parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId);

				if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)

				{
					Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddDays(1) });

					Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = sentMessage.Chat.Id, MessageId = sentMessage.MessageId, MessageExpire = DateTime.UtcNow.AddDays(1) });
				}

				return sentMessage;
			}

			static async Task<Message> Post(ITelegramBotClient botClient, Message message)
			{
				string messageToSend = "Sua postagem foi enviada para o [canal](https://t.me/CypherpunksBrasil)";

				if (message.ReplyToMessage == null)

				{
					messageToSend = "Para enviar um post para nosso canal você precisa enviar esse comando como resposta na mensagem que deseja encaminhar";
				}
				else

				{
					Data.Post postData = new Data.Post()
					{
						forwardedFrom = message.ReplyToMessage.ForwardFrom != null ? message.ReplyToMessage.ForwardFrom.Id : 0,

						sentBy = message.ReplyToMessage.From.Id,

						recomendedBy = message.From.Id,

						PostLikes = new List<Int64>(),

						PostDislikes = new List<Int64>()
					};

					Message postMessage;

					switch (message.ReplyToMessage.Type)

					{
						case Telegram.Bot.Types.Enums.MessageType.Text:

							{
								StringBuilder postText = new StringBuilder();

								postText.AppendLine(Tools.StringForkMarkdownV2Use(message.ReplyToMessage.Text));

								postText.AppendLine(Tools.StringForkMarkdownV2Use("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  - "));

								postText.AppendLine(Tools.GetChannelPostDescription(message));

								postMessage = await botClient.SendTextMessageAsync(chatId: Props.postChannelChatId, text: postText.ToString(), parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: Resources.Messages.replyMarkupChannelRating(), entities: message.Entities.ToList());

								Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);
							}

							break;

						case Telegram.Bot.Types.Enums.MessageType.Document:

							{
								StringBuilder postText = new StringBuilder();

								postText.AppendLine(Tools.StringForkMarkdownV2Use(message.ReplyToMessage.Caption));

								postText.AppendLine(Tools.StringForkMarkdownV2Use("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -"));

								postText.AppendLine(Tools.GetChannelPostDescription(message));

								postMessage = await botClient.SendDocumentAsync(chatId: Props.postChannelChatId, document: message.Document.FileId, caption: postText.ToString(), parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: Resources.Messages.replyMarkupChannelRating(), captionEntities: message.CaptionEntities);

								Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);
							}

							break;

						case Telegram.Bot.Types.Enums.MessageType.Photo:

							{
								StringBuilder postText = new StringBuilder();

								postText.AppendLine(Tools.StringForkMarkdownV2Use(message.ReplyToMessage.Caption));

								postText.AppendLine(Tools.StringForkMarkdownV2Use("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -"));

								postText.AppendLine(Tools.GetChannelPostDescription(message));

								postMessage = await botClient.SendPhotoAsync(chatId: Props.postChannelChatId, photo: message.Photo[0].FileId, caption: postText.ToString(), parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: Resources.Messages.replyMarkupChannelRating(), captionEntities: message.CaptionEntities);

								Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);
							}

							break;

						case Telegram.Bot.Types.Enums.MessageType.Video:

							{
								StringBuilder postText = new StringBuilder();

								postText.AppendLine(Tools.StringForkMarkdownV2Use(message.ReplyToMessage.Caption));

								postText.AppendLine(Tools.StringForkMarkdownV2Use("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -"));

								postText.AppendLine(Tools.GetChannelPostDescription(message));

								postMessage = await botClient.SendVideoAsync(chatId: Props.postChannelChatId, video: message.Video.FileId, caption: postText.ToString(), parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: Resources.Messages.replyMarkupChannelRating(), captionEntities: message.CaptionEntities);

								Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

							}

							break;

						case Telegram.Bot.Types.Enums.MessageType.Audio:

							{
								StringBuilder postText = new StringBuilder();

								postText.AppendLine(Tools.StringForkMarkdownV2Use(message.ReplyToMessage.Caption));

								postText.AppendLine(Tools.StringForkMarkdownV2Use("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -"));

								postText.AppendLine(Tools.GetChannelPostDescription(message));

								postMessage = await botClient.SendAudioAsync(chatId: Props.postChannelChatId, audio: message.Audio.FileId, caption: postText.ToString(), parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: Resources.Messages.replyMarkupChannelRating(), captionEntities: message.CaptionEntities);

								Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

							}

							break;

						case Telegram.Bot.Types.Enums.MessageType.Voice:

							{
								StringBuilder postText = new StringBuilder();

								postText.AppendLine(Tools.StringForkMarkdownV2Use("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -  -"));

								postText.AppendLine(Tools.GetChannelPostDescription(message));

								postMessage = await botClient.SendVoiceAsync(chatId: Props.postChannelChatId, voice: message.Voice.FileId, caption: postText.ToString(), parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, replyMarkup: Resources.Messages.replyMarkupChannelRating(), captionEntities: message.CaptionEntities);

								Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

							}

							break;

						default:

							{
								messageToSend = "Mensagem não suportada";
							}

							break;
					}
				}

				Console.WriteLine(messageToSend);

				Message sentMessage = await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: messageToSend, parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId, allowSendingWithoutReply: true, disableWebPagePreview: true, disableNotification: true);

				if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)
				{
					Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddDays(5) });

					Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = sentMessage.Chat.Id, MessageId = sentMessage.MessageId, MessageExpire = DateTime.UtcNow.AddDays(5) });
				}

				return sentMessage;
			}
			static async Task<Message> BarraComandoInutil(ITelegramBotClient botClient, Message message)
			{
				StringBuilder sb = new(Tools.StringForkMarkdownV2Use("Comando nada a ver. Vou apagar"));

				Message sentMessage = await botClient.SendTextMessageAsync(chatId: message.Chat.Id, sb.ToString(), parseMode: ParseMode.MarkdownV2, replyToMessageId: message.MessageId, allowSendingWithoutReply: true);

				Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(3) });
				Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = sentMessage.Chat.Id, MessageId = sentMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(4) });

				return sentMessage;
			}

			static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
			{
				const string usage = "Comandos:\n" +
									 "/post - responda uma mensagem com isso para postar no canal @CypherpunksBrasil.\n" +
									 "/offtopic - Link para o grupo offtopic.\n" +
									 "/help - essa mesma mensagemcom os comandos.";


				return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
															text: usage,
															parseMode: ParseMode.Markdown,
															replyMarkup: new ReplyKeyboardRemove(),
															disableWebPagePreview: true,
															disableNotification: true);
			}
		}

		// Process Inline Keyboard callback data
		private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
		{
			string txtAnswerCallbackQuery = "ok";

			if (callbackQuery == null) { return; }

			if (callbackQuery.Data == "ChannelPostLike")
			{
				if (Data.ChannelPosts.posts.ContainsKey(callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId))
				{
					if (!Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Contains(callbackQuery.From.Id))
					{
						Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Add(callbackQuery.From.Id);
						botClient.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, Resources.Messages.replyMarkupChannelRating(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId));
						txtAnswerCallbackQuery = "obrigado por opinar";
					}
					else
					{
						txtAnswerCallbackQuery = "você já votou nesse post";
					}
				}
			}

			if (callbackQuery.Data == "ChannelPostDislike")
			{
				if (Data.ChannelPosts.posts.ContainsKey(callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId))
				{
					if (!Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Contains(callbackQuery.From.Id))
					{
						Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Add(callbackQuery.From.Id);
						botClient.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, Resources.Messages.replyMarkupChannelRating(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId));
						txtAnswerCallbackQuery = "obrigado por opinar";
					}
					else
					{
						txtAnswerCallbackQuery = "você já votou nesse post";
					}
				}
			}

			await botClient.AnswerCallbackQueryAsync(
				callbackQueryId: callbackQuery.Id,
				text: txtAnswerCallbackQuery);
		}

		private static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
		{
			Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

			InlineQueryResult[] results = {
			// displayed result
			new InlineQueryResultArticle(
				id: "3",
				title: "TgBots",
				inputMessageContent: new InputTextMessageContent(
					"hello"
				)
			)
		};

			await botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
												   results: results,
												   isPersonal: true,
												   cacheTime: 0);
		}

		private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
		{
			Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
			return Task.CompletedTask;
		}

		private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
		{
			Console.WriteLine($"Unknown update type: {update.Type}");
			return Task.CompletedTask;
		}
	}
}