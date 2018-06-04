using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using AlexaAPI.Request;
using AlexaAPI.Response;
using AlexaAPI;

[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace BlackjackBuddy
{
    public class Function
    {

        private enum AppState
        {
            Start,
            Playing
        }

        AppState appstate = AppState.Start;

        const string STATE = "state";
        const string RESPONSE = "response";

        const string QUIZSTART = "quizstart";
        const string QUIZRATIO = "quizratio";
        const string QUIZBET = "quizbet";
        const string QUIZSCORE = "quizscore";

        const string COUNTER = "counter";

        const string BET = "Bet";
        const string PAYOUT = "Payout";

        const string WELCOME_MESSAGE = "Welcome to the blackjack payouts quiz.  You can ask me the blackjack payout for any bet by saying \"bet\" and the amount, or you can ask me to start a quiz.  What would you like to do?";
        const string EXIT_SKILL_MESSAGE = "Have a good day!";
        const string REPROMPT_SPEECH = "Which other bet would you like to know a payout for?";
        const string HELP_MESSAGE = "I know a lot of Blackjack payouts.  You can say, bet 10, and I will tell you the payout.  You can also test your knowledge by asking me to start a quiz.  What would you like to do?";

        static string sayas_interject = "<say-as interpret-as='interjection'>";
        static string sayas = "</say-as>";
        static string breakstrong = "<break strength='strong'/>";

        private SkillResponse response = null;
        private int counter = 0;
        private int quizscore = 0;
        private decimal quizbet = 0;
        private DateTime quizstart;

        private ILambdaContext context = null;
        private static Random rand = new Random();
        private decimal quizratio = 1.5m;

        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext ctx)
        {
            context = ctx;





            try
            {
                response = new SkillResponse();
                response.Response = new ResponseBody();
                response.Response.ShouldEndSession = false;
                response.Version = "1.0";
                
                if (input.Request.Type.Equals(AlexaConstants.LaunchRequest))
                {
                    response.Response.OutputSpeech = GetLaunchRequest();
                    appstate = AppState.Start;
                    response.SessionAttributes = new Dictionary<string, object>() { { STATE, appstate.ToString() } };
                }
                else
                {
                    if (input.Request.Type.Equals(AlexaConstants.IntentRequest))
                     {
                        if (IsDialogIntentRequest(input))
                        {
                            if (!IsDialogSequenceComplete(input))
                            {
                                CreateDelegateResponse();
                                return response;
                            }
                        }

                        response.SessionAttributes = new Dictionary<string, object>();
                        response.Response.OutputSpeech = GetIntentRequest(input);
                        response.SessionAttributes.Add(STATE, appstate.ToString());
                    }
                    else
                    {
                        if (input.Request.Type.Equals(AlexaConstants.SessionEndedRequest) &&
                            string.IsNullOrEmpty(input.Request.Reason) == false)
                        {
                            Log($"session end: " +input.Request.Reason);
                        }
                    }
                }
                Log(JsonConvert.SerializeObject(response));
                return response;
            }
            catch (Exception ex)
            {
                Log($"error :" + ex.Message);
            }

            response.SessionAttributes = new Dictionary<string, object>() { { STATE, appstate.ToString() } };
            (response.Response.OutputSpeech as PlainTextOutputSpeech).Text = HELP_MESSAGE;
            return response;
        }

        private IOutputSpeech GetLaunchRequest()
        {
            IOutputSpeech innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text = WELCOME_MESSAGE;
            appstate = AppState.Start;
            return innerResponse;
        }

        private IOutputSpeech GetIntentRequest(SkillRequest input)
        {
            var intentRequest = input.Request;
            IOutputSpeech innerResponse = new PlainTextOutputSpeech();

            switch (intentRequest.Intent.Name)
            {
                case "QuizIntent":
                    DoQuiz(input, innerResponse = new SsmlOutputSpeech());
                    break;

                case "AnswerIntent":
                    appstate = GetAppState(input);
                    AnswerQuiz(input, innerResponse = new SsmlOutputSpeech());
                    break;

                case "AnswerHalfIntent":
                    appstate = GetAppState(input);
                    AnswerQuiz(input, innerResponse = new SsmlOutputSpeech(), true);
                    break;

                case "QuestionIntent":
                    appstate = GetAppState(input);
                    AnswerFacts(input, innerResponse = new SsmlOutputSpeech());
                    break;

                case "QuestionHalfIntent":
                    appstate = GetAppState(input);
                    AnswerFacts(input, innerResponse = new SsmlOutputSpeech(), true);
                    break;

                case "Quiz":
                    appstate = GetAppState(input);
                    AnswerFacts(input, innerResponse = new SsmlOutputSpeech());
                    break;
             
                case "AskQuestion":
                    appstate = AppState.Quiz;
                    counter = GetIntAttributeProperty(input.Session.Attributes, COUNTER);
                    AskQuestion(input, innerResponse = new SsmlOutputSpeech());
                    break;

                case AlexaConstants.CancelIntent:
                    (innerResponse as PlainTextOutputSpeech).Text = EXIT_SKILL_MESSAGE;
                    response.Response.ShouldEndSession = true;
                    break;

                case AlexaConstants.StartOverIntent:
                    DoQuiz(input, innerResponse = new SsmlOutputSpeech());
                    response.Response.ShouldEndSession = false;
                    break;

                case AlexaConstants.StopIntent:
                    (innerResponse as PlainTextOutputSpeech).Text = EXIT_SKILL_MESSAGE;
                    response.Response.ShouldEndSession = true;
                    break;

                case AlexaConstants.HelpIntent:
                    (innerResponse as PlainTextOutputSpeech).Text = HELP_MESSAGE;
                    break;

                default:

                    if (appstate == AppState.Quiz)
                    {
                        AnswerQuiz(input, innerResponse = new SsmlOutputSpeech());
                    }
                    else
                    {
                        (innerResponse as PlainTextOutputSpeech).Text = WELCOME_MESSAGE;
                    }
                    break;
            }

            if (innerResponse.Type == "SSML")
            {
                (innerResponse as SsmlOutputSpeech).Ssml = "<speak>" + (innerResponse as SsmlOutputSpeech).Ssml + "</speak>";
            }

            return innerResponse;
        }

        private AppState GetAppState(SkillRequest input)
        {
            AppState ret = AppState.Start;
            string property = GetStringAttributeProperty(input.Session.Attributes, STATE);
            if (!string.IsNullOrEmpty(property) && property.Equals(AppState.Quiz.ToString()))
            {
                ret = AppState.Quiz;
            }
            return ret;
        }

        private void DoQuiz(SkillRequest input, IOutputSpeech innerResponse)
        {
            appstate = AppState.Quiz;
            ClearAppState();          
            AskQuestion(input, innerResponse);
        }

        private void ClearAppState()
        {
            counter = 0;
            quizscore = 0;
            quizbet = 0;
            quizstart = DateTime.Now;
        }

        private void AnswerFacts(SkillRequest input, IOutputSpeech innerResponse, bool half = false)
        {
            SsmlOutputSpeech output = (innerResponse as SsmlOutputSpeech);

            string textout = string.Empty;
            var intentRequest = input.Request;

            decimal payout = 0;
            decimal bet = 0;
            Item item = null;

            try
            {
                // They used the payout intent but were not in quiz mode.
                payout = decimal.Parse(intentRequest.Intent.Slots["Payout"].Value);
            }
            catch
            {
            }

            try
            {
                bet = decimal.Parse(intentRequest.Intent.Slots["Bet"].Value);
            }
            catch
            {
            }

            item = new Item((bet > 0 ? bet : payout) + (half ? .5m : 0), quizratio);

            if (item != null && item.Bet >= .5m && item.Bet <= 1000)
            {
                StandardCard card = new StandardCard();

                card.Title = GetCardTitle(item);
                card.text  = GetTextDescription(item);
                response.Response.Card = card;
                 
                output.Ssml = GetSpeechDescription(item);
                response.SessionAttributes.Add(RESPONSE, output.Ssml);

                SsmlOutputSpeech repromptResponse = new SsmlOutputSpeech();
                repromptResponse.Ssml = DecorateSsml(REPROMPT_SPEECH);
                response.Response.Reprompt = new Reprompt();
                response.Response.Reprompt.OutputSpeech = repromptResponse;
            }
            else
            {
                output.Ssml = "Please say a valid bet from .5 to 1000 dollars.";
                response.SessionAttributes.Add(RESPONSE, output.Ssml);
                response.Response.Reprompt = new Reprompt();
                response.Response.Reprompt.OutputSpeech = innerResponse;
            }
        }

        private void AnswerQuiz(SkillRequest input, IOutputSpeech innerResponse, bool half = false)
        {
            var intentRequest = input.Request;

            try
            {
                counter = GetIntAttributeProperty(input.Session.Attributes, COUNTER);
                quizscore = GetIntAttributeProperty(input.Session.Attributes, QUIZSCORE);
                quizbet = decimal.Parse(GetStringAttributeProperty(input.Session.Attributes, QUIZBET));
                quizstart = DateTime.Parse(GetStringAttributeProperty(input.Session.Attributes, QUIZSTART));
            }
            catch
            {
                AnswerFacts(input, innerResponse, half);
                return;
            }

            decimal payout = 0;
            Item item = null;
            try
            {
                payout = decimal.Parse(intentRequest.Intent.Slots["Payout"].Value) + (half ? .5m : 0);
                item = new Item(quizbet, quizratio);
            }
            catch(Exception ex)
            {
                Log(ex.Message);
            }

            if (item != null)
            {
                if (item.Payout == payout)
                {
                    quizscore++;
                    (innerResponse as SsmlOutputSpeech).Ssml = GetSpeechCon(true);
                }
                else
                {
                    (innerResponse as SsmlOutputSpeech).Ssml = GetSpeechCon(false);
                }

                (innerResponse as SsmlOutputSpeech).Ssml += GetAnswer(item);
                if (counter < MAX_QUESTION)
                {
                    (innerResponse as SsmlOutputSpeech).Ssml += GetCurrentScore(quizscore, counter);
                    AskQuestion(input, innerResponse);
                }
                else
                {
                    (innerResponse as SsmlOutputSpeech).Ssml += GetFinalScore(quizscore, counter, quizstart);
                    (innerResponse as SsmlOutputSpeech).Ssml += " " + EXIT_SKILL_MESSAGE;
                    response.SessionAttributes.Add(RESPONSE, (innerResponse as SsmlOutputSpeech).Ssml);
                    appstate = AppState.Start;
                    ClearAppState();
                }
            }
            else
            {
                string question = GetQuestion(counter, quizbet);
                (innerResponse as SsmlOutputSpeech).Ssml += question;

                response.Response.Reprompt = new Reprompt();
                response.Response.Reprompt.OutputSpeech = new SsmlOutputSpeech();
                (response.Response.Reprompt.OutputSpeech as SsmlOutputSpeech).Ssml = DecorateSsml(question);
                response.SessionAttributes.Add(RESPONSE, (innerResponse as SsmlOutputSpeech).Ssml);
            }
        }

        private void AskQuestion(SkillRequest input, IOutputSpeech innerResponse)
        {
            if (counter <= 0)
            {
                (innerResponse as SsmlOutputSpeech).Ssml = START_QUIZ_MESSAGE + " ";
                counter = 0;
            }

            counter++;
            response.SessionAttributes.Add(COUNTER, counter);

            decimal bet = Item.Tests[GetRandomNumber(0, Item.Tests.Length - 1)];

            response.SessionAttributes.Add(QUIZBET, bet.ToString());
            response.SessionAttributes.Add(QUIZSCORE, quizscore);
            response.SessionAttributes.Add(QUIZSTART, quizstart.ToString());

            string question = GetQuestion(counter, bet);
            (innerResponse as SsmlOutputSpeech).Ssml += question;

            response.Response.Reprompt = new Reprompt();
            response.Response.Reprompt.OutputSpeech = new SsmlOutputSpeech();
            (response.Response.Reprompt.OutputSpeech as SsmlOutputSpeech).Ssml = DecorateSsml(question); 
            response.SessionAttributes.Add(RESPONSE, (innerResponse as SsmlOutputSpeech).Ssml);
        }

        static string DecorateSsml(string instr)
        {
            return "<speak>" + instr + "</speak>";
        }

        string GetStringAttributeProperty (Dictionary <string, object> property, string key) 
        {
            if (property != null)
            {
                if (property.ContainsKey(key))
                {
                    return (string)property[key];
                }
            }
            return string.Empty;
        }

        int GetIntAttributeProperty(Dictionary<string, object> property, string key)
        {
            if (property != null)
            {
                if (property.ContainsKey(key))
                {
                    try
                    {
                        Int64 i = (Int64)property[key];
                        return (int) i;
                    }
                    catch (Exception ex)
                    {
                        Log("getIntAttributeProperty " + ex.Message);
                    }
                }
            }
            return -1;
        }

        int GetDecimalAttributeProperty(Dictionary<string, object> property, string key)
        {
            if (property != null)
            {
                if (property.ContainsKey(key))
                {
                    try
                    {
                        Int64 i = (Int64)property[key];
                        return (int)i;
                    }
                    catch (Exception ex)
                    {
                        Log("getIntAttributeProperty " + ex.Message);
                    }
                }
            }
            return -1;
        }

        private string GetSpeechDescription(Item item)
        {
            return item.Payout + " is the payout for a bet of " + item.Bet.ToString().Replace(".0", "") + ".  Which other bet would you like to know about?";
        }

        private string GetQuestion(int counter, decimal bet)
        {
            return "Here is question " + counter.ToString() + ".  What is the payout for "  + bet.ToString().Replace(".0", "") + " dollars.";
        }

        private string GetAnswer(Item item)
        {
            return "The payout for " + item.Bet.ToString().Replace(".0", "") + " is " + item.Payout.ToString().Replace(".0", "") + ". ";                       
        }

        private string GetBadAnswer(string item)
        {
            return "go on";
        }

        private void CreateDelegateResponse()
        {
            DialogDirective dld = new DialogDirective()
            {
                Type = AlexaConstants.DialogDelegate
            };
            response.Response.Directives.Add(dld);
        }

        private bool IsDialogIntentRequest(SkillRequest input)
        {
            if (string.IsNullOrEmpty(input.Request.DialogState))
                return false;

            return true;
        }

        private bool IsDialogSequenceComplete(SkillRequest input)
        {
           if (input.Request.DialogState.Equals(AlexaConstants.DialogCompleted))
           {
              return true;
           }

           return false;
        }


        private string GetTextDescription(Item item)
        {
            return Item.GetFormatedText(item);
        }

        private string GetSpeechCon(bool type)
        {
            if (type)
            {
                return sayas_interject + "Correct! " + sayas + breakstrong;
            }
            return sayas_interject + "Incorrect " + sayas + breakstrong;
        }
        
        private string GetCurrentScore(int score, int counter)
        {
            return "Your current score is " + score.ToString() + " out of " + counter.ToString() + ". ";
        }

        private string GetFinalScore(int score, int counter, DateTime start)
        {
            int seconds = (int)(DateTime.Now.Subtract(start).TotalSeconds);
            int minutes = seconds / 60;
            int secs = seconds - (minutes * 60);
           return "Your final score is " + score.ToString() + " out of " + counter.ToString() + " in " + minutes + " minutes and " + secs + " seconds. ";
        }
    
        private string GetCardTitle(Item item)
        {
            return "Bet of " + item.Bet;
        }

        string START_QUIZ_MESSAGE = "OK.  This quiz will ask payouts for " + MAX_QUESTION.ToString() + " bets.";
        
        private void Log(string text)
        {
            if (context != null)
            {
                context.Logger.LogLine(text);
            }
        }
    }
}