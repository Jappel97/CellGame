using CellGame.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CellGame.Classes
{
    public static class AnswerDatabaseUtils
    {
        /// <summary>
        /// adds an answer to the database when someone adds an answer to a question when editing a scenario
        /// </summary>
        /// <param name="answer">the answer that they have modified in the UI</param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static int addAnswer(Answer answer, SqlConnection connection)
        {
            try
            {
                var addAnswerCommand = new SqlCommand(null, connection)
                {
                    CommandText = "INSERT INTO ANSWERS (ANSWER_FOR_QUESTION, ANSWER_TEXT, ANSWER_IS_ACTIVE, ANSWER_REQUIRES_COMMENT) " +
                                    "VALUES (@answerForQuestion, @answerText, 1, @requiresComment); SELECT SCOPE_IDENTITY();"
                };

                var param = new SqlParameter("@answerForQuestion", SqlDbType.Int)
                {
                    Value = answer.answerForQuestion
                };
                addAnswerCommand.Parameters.Add(param);

                var param2 = new SqlParameter("@answerText", SqlDbType.VarChar, 500)
                {
                    Value = answer.answerText
                };
                addAnswerCommand.Parameters.Add(param2);

                var param3 = new SqlParameter("@requiresComment", SqlDbType.Bit)
                {
                    Value = answer.requiresComment
                };
                addAnswerCommand.Parameters.Add(param3);


                connection.Close(); connection.Open();
                addAnswerCommand.Prepare();
                answer.answerID = Convert.ToInt32(addAnswerCommand.ExecuteScalar());

            }
            catch(Exception ex)
            {
                Singleton.errorCode = "ANS01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return answer.answerID;
        }

        /// <summary>
        /// Updates the answer from the EditScenario UI
        /// </summary>
        /// <param name="answer">the answer that they have modified in the UI</param>
        /// <param name="connection">connection to the database</param>
        public static void editAnswer(Answer answer, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE ANSWERS SET " +
                                  "ANSWER_FOR_QUESTION = @answerForQuestion, " +
                                  "ANSWER_TEXT = @answerText, " +
                                  "ANSWER_REQUIRES_COMMENT = @requiresComment " +
                                  "WHERE ANSWER_ID = @answerID"
                };

                var param = new SqlParameter("@answerForQuestion", SqlDbType.Int)
                {
                    Value = answer.answerForQuestion
                };
                command.Parameters.Add(param);

                var param2 = new SqlParameter("@answerText", SqlDbType.VarChar, 500)
                {
                    Value = answer.answerText
                };
                command.Parameters.Add(param2);

                var param3 = new SqlParameter("@answerID", SqlDbType.Int)
                {
                    Value = answer.answerID
                };
                command.Parameters.Add(param3);

                var param4 = new SqlParameter("@requiresComment", SqlDbType.Bit)
                {
                    Value = Convert.ToInt32(answer.requiresComment)
                };
                command.Parameters.Add(param4);

                connection.Close(); connection.Open();
                command.Prepare();
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Singleton.errorCode = "ANS02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// get the count of how many times an answer is used within a scenario. This is used to know if we can delete/make active/make inactive
        /// we can't make an answer active if the question it is in is inactive
        /// </summary>
        /// <param name="answerID">the selected answer object in the UI to modify the answer based off the ID in the database </param>
        /// <param name="connection">connection to the database</param>
        /// <returns></returns>
        public static int getAnswerUsage(int answerID, SqlConnection connection)
        {
            int count;
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT COUNT(*) FROM STUDENT_ANSWERS WHERE SA_ANSWER_ID = @answerID"
                };

                var param = new SqlParameter("@answerID", SqlDbType.Int)
                {
                    Value = answerID
                };
                command.Parameters.Add(param);

                connection.Close(); connection.Open();
                command.Prepare();

                count = (Int32) command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "ANS03";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }

            return count;
        }

        /// <summary>
        /// make a answer inactive
        ///    This means it still exists but cannot be seen when playing a scenario
        /// </summary>
        /// <param name="answerID">the selected answer object in the UI to modify the answer based off the ID in the database </param>
        /// <param name="connection">connection to the database</param>
        public static void makeAnswerInactive(int answerID, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE ANSWERS SET ANSWER_IS_ACTIVE = 0 WHERE ANSWER_ID = @answerID"
                };

                var param = new SqlParameter("@answerID", SqlDbType.Int)
                {
                    Value = answerID
                };
                command.Parameters.Add(param);

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "ANS04";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// make a answer active
        ///    This means it can be seen while playing
        /// </summary>
        /// <param name="answerID">the selected answer object in the UI to modify the answer based off the ID in the database </param>
        /// <param name="connection">connection to the database</param>
        public static bool makeAnswerActive(int answerID, SqlConnection connection)
        {
            try
            {
                var checkForQuestionInactiveCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT QUESTIONS.QUESTION_IS_ACTIVE FROM QUESTIONS " +
                                  "WHERE QUESTION_ID IN (SELECT ANSWER_QUESTIONS.QUESTION_REFERENCE_ID FROM " +
                                                    "ANSWER_QUESTIONS WHERE ANSWER_QUESTIONS.ANSWER_ID = @answerID);"
                };
                var answerIDParam = new SqlParameter("@answerID", SqlDbType.Int)
                {
                    Value = answerID
                };
                checkForQuestionInactiveCommand.Parameters.Add(answerIDParam);

                connection.Close(); connection.Open();
                checkForQuestionInactiveCommand.Prepare();
                using (SqlDataReader reader = checkForQuestionInactiveCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ($"{reader["QUESTION_IS_ACTIVE"]}" == "False")
                        {
                            Singleton.errorMsg = "Unable to activate this question. One or more questions this answer references are inactive. ";
                            Singleton.isError = true;
                            return false;
                        }
                    }
                }
                checkForQuestionInactiveCommand.Parameters.Clear();
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE ANSWERS SET ANSWER_IS_ACTIVE = 1 WHERE ANSWER_ID = @answerID"
                };


                command.Parameters.Add(answerIDParam);

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "ANS05";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return true;
        }

        /// <summary>
        ///    This deletes an answer.
        ///         To delete the answer must not have been answered in a Play Scenario and inserted into a grade
        /// </summary>
        /// <param name="answerID">the selected answer object in the UI to modify the answer based off the ID in the database </param>
        /// <param name="connection">connection to the database</param>
        public static bool deleteAnswer(int answerID, SqlConnection connection)
        {
            try
            {
                var checkIfReferencesCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT COUNT(*) FROM STUDENT_ANSWERS WHERE SA_ANSWER_ID = 2;"
                };

                var answerIDParam = new SqlParameter("@answerID", SqlDbType.Int)
                {
                    Value = answerID
                };

                checkIfReferencesCommand.Parameters.Add(answerIDParam);
                connection.Close(); connection.Open();

                if ((Int32)checkIfReferencesCommand.ExecuteScalar() > 0)
                {
                    Singleton.warningMsg =
                        "This answer is referenced in current answers that are being graded, so it has been inactivated";
                    return false;
                }

                checkIfReferencesCommand.Parameters.Clear();
                var deletAnswerCommand = new SqlCommand(null, connection)
                {
                    CommandText = "DELETE FROM ANSWER_QUESTIONS WHERE ANSWER_ID = @answerID; " + 
                                        "DELETE FROM ANSWERS WHERE ANSWER_ID = @answerID;"
                };
                deletAnswerCommand.Parameters.Add(answerIDParam);

                connection.Close(); connection.Open();
                deletAnswerCommand.Prepare();

                deletAnswerCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "ANS06";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return true;
        }

        /// <summary>
        /// deleting answer references deletes the questions that an answer points to
        /// </summary>
        /// <param name="answerID">the answers whose references you want to delete</param>
        /// <param name="questionReferenceID">the question reference you want to remove from an answer</param>
        /// <param name="connection">database connection</param>
        public static void deleteAnswerReference(int answerID, int questionReferenceID, SqlConnection connection)
        {
            try
            {
                var deleteQuestionReferencesCommand = new SqlCommand(null, connection)
                {
                    CommandText = "DELETE FROM ANSWER_QUESTIONS WHERE ANSWER_ID = @answerID " + 
                                        "AND QUESTION_REFERENCE_ID = @questionID "
                };
                var paramAnswerID = new SqlParameter("@answerID", SqlDbType.Int)
                {
                    Value = answerID
                };
                var paramQuestionID = new SqlParameter("@questionID", SqlDbType.Int)
                {
                    Value = questionReferenceID
                };

            

                deleteQuestionReferencesCommand.Parameters.Add(paramAnswerID);
                deleteQuestionReferencesCommand.Parameters.Add(paramQuestionID);

                connection.Close(); connection.Open();
                deleteQuestionReferencesCommand.Prepare();
                deleteQuestionReferencesCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "ANS07";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Edit a question reference
        /// </summary>
        /// <param name="answerID">answer where you want to update the referece</param>
        /// <param name="newReference">the edited reference</param>
        /// <param name="connection">database connection</param>
        internal static void editAnswerReference(int answerID, QuestionReference newReference, SqlConnection connection)
        {
            try
            {
                SqlCommand updateQuestionReferenceCommand = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE ANSWER_QUESTIONS SET QUESTION_REFERENCE_ID = @newReferenceID, " +
                                    "PROBABILITY = @probability WHERE ANSWER_ID = @answerID " +
                                    "AND QUESTION_REFERENCE_ID = @prevReferenceID;"
                };
                var paramAnswerID = new SqlParameter("@answerID", SqlDbType.Int)
                {
                    Value = answerID
                };
                var paramPrevReferenceID = new SqlParameter("@prevReferenceID", SqlDbType.Int)
                {
                    Value = newReference.prevQuestionReference
                };
                var paramNewReferenceID = new SqlParameter("@newReferenceID", SqlDbType.Int)
                {
                    Value = newReference.questionReference
                };
                var paramProbability = new SqlParameter("@probability", SqlDbType.Int)
                {
                    Value = newReference.questionReferenceProbability
                };


                updateQuestionReferenceCommand.Parameters.Add(paramAnswerID);
                updateQuestionReferenceCommand.Parameters.Add(paramPrevReferenceID);
                updateQuestionReferenceCommand.Parameters.Add(paramNewReferenceID);
                updateQuestionReferenceCommand.Parameters.Add(paramProbability);

                connection.Close(); connection.Open();
                updateQuestionReferenceCommand.Prepare();
                updateQuestionReferenceCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "ANS08";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        ///  add reference to a question from an answer (an answer points to a question or multiple questions)
        /// </summary>
        /// <param name="answerID">answer you want to attach the reference to</param>
        /// <param name="questionReference">reference you want to add to the answer</param>
        /// <param name="connection">database connection</param>
        public static void addAnswerReference(int answerID, QuestionReference questionReference, SqlConnection connection)
        {
            try
            {
                SqlCommand addQuestionReferenceCommand = new SqlCommand(null, connection)
                {
                    CommandText = "INSERT INTO ANSWER_QUESTIONS (ANSWER_ID, QUESTION_REFERENCE_ID, PROBABILITY) " +
                                    "VALUES (@answerID, @questionID, @probability);"
                };

                var paramID = new SqlParameter("@answerID", SqlDbType.Int)
                {
                    Value = answerID
                };
                var paramQuestion = new SqlParameter("@questionID", SqlDbType.Int)
                {
                    Value = questionReference.questionReference
                };
                var paramProbability = new SqlParameter("@probability", SqlDbType.Int)
                {
                    Value = questionReference.questionReferenceProbability
                };

                addQuestionReferenceCommand.Parameters.Add(paramID);
                addQuestionReferenceCommand.Parameters.Add(paramQuestion);
                addQuestionReferenceCommand.Parameters.Add(paramProbability);

                connection.Close(); connection.Open();
                addQuestionReferenceCommand.Prepare();
                addQuestionReferenceCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "ANS09";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }

        }

    }
}