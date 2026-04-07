// Global using directives

global using System.Diagnostics;
global using System.Text.Json;
global using System.Web;
global using MassTransit;
global using MassTransit.Mediator;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using FpvLadderBot.BackNavigations;
global using FpvLadderBot.Commands;
global using FpvLadderBot.Events;
global using FpvLadderBot.Events.CommandReceivedConsumers.Base;
global using FpvLadderBot.Models;
global using FpvLadderBot.PipelineStateMachine;
global using FpvLadderBot.Queries;
global using FpvLadderBot.Subscriptions;
global using FpvLadderBot.Utils;
global using Telegram.Bot;
global using Telegram.Bot.Polling;
global using Telegram.Bot.Types;
global using Telegram.Bot.Types.Enums;
global using Telegram.Bot.Types.ReplyMarkups;
