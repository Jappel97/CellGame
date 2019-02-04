using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using CellGame.Classes;
using CellGame.Models;

namespace CellGame.Classes
{
    /*
     * This class contains all the functions for interacting with the scenarios in the database.
     */
    public static class ScenarioDatabaseUtils
    {

        /*
         * This function takes in a Scenario model and inserts it into the database. This only inserts the
         * scenario information, not any questions and answers that might be contained in the scenario.
         * By default the scenario is not approved. This returns the ID for the inserted scenario.
         */
        public static int addScenario(Scenario scenario, SqlConnection connection)
        {
            try
            {
                SqlCommand commandInsertScenario = new SqlCommand(null, connection)
                {
                    CommandText = "INSERT INTO SCENARIOS " +
                                  "(" +
                                  "SCENARIO_NAME, " +
                                  "SCENARIO_DESCRIPTION, " +
                                  "SCENARIO_APPROVED, " +
                                  "SCENARIO_MADE_BY, " +
                                  "SCENARIO_CELL_FUNCTION, " +
                                  "SCENARIO_CELL_SHAPE_AND_FEATURES, " +
                                  "SCENARIO_CELL_LIFESPAN, " +
                                  "SCENARIO_CELL_NUTRITION " +
                                  ") " +
                                  "VALUES " +
                                  "(" +
                                  "@scenarioName, " +
                                  "@scenarioDescription, " +
                                  "0, " +
                                  "@scenarioBy," +
                                  "@cellFunction," +
                                  "@cellShapeAndFeatures," +
                                  "@cellLifespan," +
                                  "@cellNutrition " +
                                  "); SELECT SCOPE_IDENTITY();"
                };

                commandInsertScenario.Parameters.Add(new SqlParameter("@scenarioName", SqlDbType.VarChar, 100)
                {
                    Value = scenario.scenarioName
                });

                commandInsertScenario.Parameters.Add(new SqlParameter("@scenarioDescription", SqlDbType.VarChar, 1000)
                {
                    Value = scenario.description
                });
                commandInsertScenario.Parameters.Add(new SqlParameter("@scenarioBy", SqlDbType.NVarChar, 128)
                {
                    Value = (string)HttpContext.Current.Session["userId"]
                });
                commandInsertScenario.Parameters.Add(new SqlParameter("@cellFunction", SqlDbType.NVarChar, 1000)
                {
                    Value = scenario.cellFunction
                });
                commandInsertScenario.Parameters.Add(new SqlParameter("@cellShapeAndFeatures", SqlDbType.NVarChar, 1000)
                {
                    Value = scenario.cellShapeAndFeatures
                });
                commandInsertScenario.Parameters.Add(new SqlParameter("@cellLifespan", SqlDbType.NVarChar, 1000)
                {
                    Value = scenario.cellLifespan
                });
                commandInsertScenario.Parameters.Add(new SqlParameter("@cellNutrition", SqlDbType.NVarChar, 1000)
                {
                    Value = scenario.cellNutrition
                });


                connection.Close(); connection.Open();
                commandInsertScenario.Prepare();

                scenario.scenarioID = Convert.ToInt32(commandInsertScenario.ExecuteScalar());
                connection.Close();
                return scenario.scenarioID;
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes in a scenario and updates it to the new values.
         */

        public static void editScenario(Scenario scenario, SqlConnection connection)
        {
            try
            {
                SqlCommand command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE SCENARIOS " +
                                  "SET SCENARIO_NAME = @scenarioName," +
                                  "SCENARIO_DESCRIPTION = @scenarioDescription, " +
                                  "SCENARIO_APPROVED = 0, " +
                                  "SCENARIO_CELL_FUNCTION = @cellFunction, " +
                                  "SCENARIO_CELL_SHAPE_AND_FEATURES = @cellShapeAndFeatures, " +
                                  "SCENARIO_CELL_LIFESPAN = @cellLifespan, " +
                                  "SCENARIO_CELL_NUTRITION = @cellNutrition " +
                                  "WHERE SCENARIO_ID = @scenarioID"
                };

                command.Parameters.Add(new SqlParameter("@scenarioName", SqlDbType.VarChar, 100)
                {
                    Value = scenario.scenarioName
                });
                command.Parameters.Add(new SqlParameter("@scenarioDescription", SqlDbType.VarChar, 1000)
                {
                    Value = scenario.description
                });
                command.Parameters.Add(new SqlParameter("@scenarioID", SqlDbType.Int)
                {
                    Value = scenario.scenarioID
                });
                command.Parameters.Add(new SqlParameter("@cellFunction", SqlDbType.VarChar, 1000)
                {
                    Value = scenario.cellFunction
                });
                command.Parameters.Add(new SqlParameter("@cellShapeAndFeatures", SqlDbType.VarChar, 1000)
                {
                    Value = scenario.cellShapeAndFeatures
                });
                command.Parameters.Add(new SqlParameter("@cellLifespan", SqlDbType.VarChar, 1000)
                {
                    Value = scenario.cellLifespan
                });
                command.Parameters.Add(new SqlParameter("@cellNutrition", SqlDbType.VarChar, 1000)
                {
                    Value = scenario.cellNutrition
                });

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes in a scenario ID and deletes that entry in the scenarios table in the database.
         */

        public static void deleteScenario(int scenarioID, SqlConnection connection)
        {
            try
            {
                SqlCommand command = new SqlCommand(null, connection)
                {
                    CommandText = "DELETE FROM SCENARIOS WHERE SCENARIO_ID = @scenarioID"
                };

                command.Parameters.Add(new SqlParameter("@scenarioID", SqlDbType.VarChar, 15)
                {
                    Value = scenarioID
                });

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN03";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function allows teachers and admins to delete ungraded scenario attempts
         */
        public static void deleteGrade(int stgID, SqlConnection connection)
        {
            try
            {
                clearGrade(connection, stgID);
                SqlCommand command = new SqlCommand(null, connection)
                {
                    CommandText = "DELETE FROM SCENARIOS_TO_GRADE WHERE STG_ID = @stgID"
                };

                command.Parameters.Add(new SqlParameter("@stgID", SqlDbType.VarChar, 15)
                {
                    Value = stgID
                });

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN14";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Removes a grade from the database
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="stgID"></param>
        private static void clearGrade(SqlConnection connection, int stgID)
        {
            try
            {
                SqlCommand command = new SqlCommand(null, connection)
                {
                    CommandText = "DELETE FROM STUDENT_ANSWERS WHERE SA_SCENARIO_TO_GRADE = @stgID"
                };

                command.Parameters.Add(new SqlParameter("@stgID", SqlDbType.VarChar, 15)
                {
                    Value = stgID
                });
                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch(Exception ex)
            {
                Singleton.errorCode = "SEN14";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes a scenario ID and pulls the information about that scenario from the database
         * and returns that scenario.
         */

        public static Scenario getScenario(int scenarioId, SqlConnection connection)
        {
            Scenario scenario = new Scenario();

            try
            {
                SqlCommand scenarioCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT * " +
                                  "FROM SCENARIOS " +
                                  "WHERE SCENARIO_ID = @id " +
                                  "AND SCENARIO_ID != 0 " + 
                                  "ORDER BY SCENARIO_IS_ACTIVE ASC"
                };

                // Create and prepare an SQL statement.
                SqlParameter param1 = new SqlParameter("@id", SqlDbType.Int)
                {
                    Value = scenarioId
                };
                scenarioCommand.Parameters.Add(param1);
                // Call Prepare after setting the Commandtext and Parameters.

                connection.Close(); connection.Open();
                scenarioCommand.Prepare();
                using (SqlDataReader reader = scenarioCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        scenario.scenarioName = $"{reader["SCENARIO_NAME"]}";
                        scenario.scenarioID = Convert.ToInt32($"{reader["SCENARIO_ID"]}");
                        scenario.description = $"{reader["SCENARIO_DESCRIPTION"]}";
                        scenario.isActive = Convert.ToBoolean($"{reader["SCENARIO_IS_ACTIVE"]}");
                        scenario.cellFunction = $"{reader["SCENARIO_CELL_FUNCTION"]}";
                        scenario.cellShapeAndFeatures = $"{reader["SCENARIO_CELL_SHAPE_AND_FEATURES"]}";
                        scenario.cellLifespan = $"{reader["SCENARIO_CELL_LIFESPAN"]}";
                        scenario.cellNutrition = $"{reader["SCENARIO_CELL_NUTRITION"]}";
                        try
                        {
                            scenario.currentQuestion = Convert.ToInt32($"{reader["SCENARIO_FIRST_QUESTION"]}");
                        }
                        catch (Exception ex)
                        {
                            Singleton.errorCode = "SEN04";
                            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                            scenario.currentQuestion = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN04";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }

            return scenario;
        }


        /*
         * This function returns the number of times that a scenario is present in the grades table.
         * It is used when an admin or teacher attempts to delete a scenario to let them know how
         * many references there are to the scenario.
         */

        public static int getScenarioUsage(int scenarioID, SqlConnection connection)
        {
            int count;
            try
            {
                SqlCommand command = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT COUNT(*) FROM SCENARIOS_TO_GRADE WHERE STG_SCENARIO_ID = @scenarioID"
                };

                command.Parameters.Add(new SqlParameter("@scenarioID", SqlDbType.Int)
                {
                    Value = scenarioID
                });

                connection.Close(); connection.Open();
                command.Prepare();

                count = (int)command.ExecuteScalar();

                SqlCommand command2 = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT COUNT(*) FROM QUESTIONS WHERE QUESTION_SCENARIO = @scenarioID"
                };

                command2.Parameters.Add(new SqlParameter("@scenarioID", SqlDbType.Int)
                {
                    Value = scenarioID
                });

                count += (int)command2.ExecuteScalar();

                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN05";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return count;
        }


        /*
         * This function takes in a scenario ID and makes that scenario inactive. Scenarios may not be able to 
         * be deleted when they are referenced, but this makes them inactive so they are not visible.
         */
        public static void makeScenarioInactive(int scenarioID, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE SCENARIOS SET SCENARIO_IS_ACTIVE = 0 WHERE SCENARIO_ID = @scenarioID"
                };

                var param = new SqlParameter("@scenarioID", SqlDbType.Int)
                {
                    Value = scenarioID
                };
                command.Parameters.Add(param);

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN06";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function approves the scenario that matches the scenario ID
         */

        public static void approveScenario(int scenarioId, SqlConnection connection)
        {
            try
            {
                SqlCommand approveScenarioCommand = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE SCENARIOS " +
                                  "SET SCENARIO_APPROVED = 1 " +
                                  "WHERE SCENARIO_ID = @id;"
                };

                // Create and prepare an SQL statement.
                SqlParameter param1 = new SqlParameter("@id", SqlDbType.Int)
                {
                    Value = scenarioId
                };
                approveScenarioCommand.Parameters.Add(param1);
                // Call Prepare after setting the Commandtext and Parameters.

                connection.Close(); connection.Open();
                approveScenarioCommand.Prepare();

                approveScenarioCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN07";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }

        }

        /*
         * This function takes in a boolean value and returns a list of scenarios where the approved
         * status of the scenario matches that of the input value.
         */

        public static Scenarios getScenarios(bool approved, SqlConnection connection)
        {
            var scenarios = new Scenarios { ScenarioList = new List<Scenario>() };
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT * " +
                                  "FROM SCENARIOS JOIN STUDENTS ON SCENARIOS.SCENARIO_MADE_BY = STUDENTS.STUDENT_ID " +
                                  "WHERE SCENARIO_ID != 0 AND SCENARIO_APPROVED = 1"
                };
                if (!approved)
                {
                    command.CommandText = "SELECT * " +
                                            "FROM SCENARIOS JOIN STUDENTS ON SCENARIOS.SCENARIO_MADE_BY = STUDENTS.STUDENT_ID " +
                                            "WHERE SCENARIO_ID != 0 AND SCENARIO_APPROVED = 0";

                }

                // Create and prepare an SQL statement.
                // Call Prepare after setting the Commandtext and Parameters.
                connection.Close(); connection.Open();
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var scenario = new Scenario();
                        scenario.scenarioName = $"{reader["SCENARIO_NAME"]}";
                        scenario.scenarioID = Convert.ToInt32($"{reader["SCENARIO_ID"]}");
                        scenario.description = $"{reader["SCENARIO_DESCRIPTION"]}";
                        scenario.isActive = Convert.ToBoolean($"{reader["SCENARIO_IS_ACTIVE"]}".ToLower());
                        scenario.madeBy = $"{reader["STUDENT_NAME"]}";
                        scenario.cellFunction = $"{reader["SCENARIO_CELL_FUNCTION"]}";
                        scenario.cellShapeAndFeatures = $"{reader["SCENARIO_CELL_SHAPE_AND_FEATURES"]}";
                        scenario.cellLifespan = $"{reader["SCENARIO_CELL_LIFESPAN"]}";
                        scenario.cellNutrition = $"{reader["SCENARIO_CELL_NUTRITION"]}";
                        if ($"{reader["SCENARIO_FIRST_QUESTION"]}".Equals(""))
                            scenario.currentQuestion = 0;
                        else
                            scenario.currentQuestion = Convert.ToInt32($"{reader["SCENARIO_FIRST_QUESTION"]}");
                        scenarios.ScenarioList.Add(scenario);
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN08";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }

            return scenarios;
        }

        /*
         * This function takes in a scenario and sets the Singleton.currentScenario to this scenario.
         * It pulls out and populates all the questions and answers associated with that scenario and stores
         * them in the respective data elements. When looping through potential question references, it will 
         * select one of them randomly according to the probabalities listed and set that reference as the next question.
         */
        public static void getQuestionsAndAnswers(Scenario scenario, SqlConnection connection)
        {
            try
            {
                var questionCommand = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.

                questionCommand.CommandText =
                    "SELECT * FROM QUESTIONS LEFT OUTER JOIN ANSWERS ON QUESTIONS.QUESTION_ID = ANSWERS.ANSWER_FOR_QUESTION " +
                            "LEFT OUTER JOIN ANSWER_QUESTIONS ON ANSWERS.ANSWER_ID = ANSWER_QUESTIONS.ANSWER_ID " +
                            "WHERE QUESTION_SCENARIO = @id AND QUESTIONS.QUESTION_ID != 0 AND QUESTIONS.QUESTION_ID != -1";

                var param2 = new SqlParameter("@id", SqlDbType.Int);
                param2.Value = scenario.scenarioID;
                questionCommand.Parameters.Add(param2);
                // Call Prepare after setting the Commandtext and Parameters.

                int previousAnswerID = -1;
                int previousQuestionID = -1;

                scenario.questions = new Dictionary<int, Question>();
                scenario.questionsActive = new Dictionary<int, Question>();
                connection.Close(); connection.Open();
                questionCommand.Prepare();
                using (var reader = questionCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // each row contains a question, answer, and answer reference

                        // if the question in a given row is new, record the values and store that question in the
                        // list of questions that belongs to the scenario
                        var questionLocation = Convert.ToInt32($"{reader["QUESTION_ID"]}");
                        if (!scenario.questions.ContainsKey(questionLocation))
                        {
                            if (previousQuestionID != -1)
                            {
                                if (scenario.questions[previousQuestionID].isActive)
                                {
                                    scenario.questionsActive.Add(previousQuestionID, scenario.questions[previousQuestionID]);
                                }

                            }
                            var question = new Question();
                            question.questionTitle = $"{reader["QUESTION_TITLE"]}";
                            question.questionScenario = Convert.ToInt32($"{reader["QUESTION_SCENARIO"]}");
                            question.questionDescription = $"{reader["QUESTION_DESCRIPTION"]}";
                            question.questionPicture = $"{reader["QUESTION_PICTURE_URL"]}";
                            question.questionId = questionLocation;
                            question.isActive = Convert.ToBoolean($"{reader["QUESTION_IS_ACTIVE"]}");
                            question.answerList = new Dictionary<int, Answer>();
                            scenario.questions.Add(questionLocation, question);
                        }

                        int answerID = 0;
                        string answerIdString = $"{reader["ANSWER_ID"]}";
                        if (!answerIdString.Equals(""))
                        {
                            answerID = Convert.ToInt32(answerIdString);

                            // if the answer in a given row is new, record the values and store it as an answer for the current question
                            if (!scenario.questions[questionLocation].answerList.ContainsKey(answerID))
                            {
                                // if this is a new answer but not the first one, go through the question references for the previous answer
                                // and set the value for one of them as the selected question reference.
                                if (previousAnswerID != -1 &&
                                    scenario.questions[previousQuestionID].answerList.Count != 0)
                                {
                                    if(scenario.questions[previousQuestionID].answerList[previousAnswerID]
                                        .questionReferences.Count != 0)
                                    {
                                        List<int> cdf = new List<int>();
                                        int totalProbability = 0;

                                        foreach (QuestionReference reference in scenario.questions[previousQuestionID]
                                            .answerList[previousAnswerID].questionReferences)
                                        {
                                            totalProbability += reference.questionReferenceProbability;
                                            cdf.Add(totalProbability);
                                        }
                                        Random r = new Random();
                                        int questionProbabilityNumber = r.Next(0, totalProbability);

                                        for (int i = 0;
                                            i < scenario.questions[previousQuestionID].answerList[previousAnswerID]
                                                .questionReferences.Count;
                                            i++)
                                        {
                                            if (questionProbabilityNumber <= cdf[i])
                                            {
                                                scenario.questions[previousQuestionID].answerList[previousAnswerID]
                                                    .nextQuestion = scenario.questions[previousQuestionID]
                                                    .answerList[previousAnswerID].questionReferences[i].questionReference;
                                                break;
                                            }
                                        }
                                    }
                                }

                                var answer = new Answer();
                                answer.answerID = answerID;
                                answer.answerText = $"{reader["ANSWER_TEXT"]}";
                                string answerForQuestionString = $"{reader["ANSWER_FOR_QUESTION"]}";
                                var isActive = $"{reader["ANSWER_IS_ACTIVE"]}";
                                if (!String.IsNullOrEmpty(isActive))
                                    answer.isActive = Convert.ToBoolean(isActive.ToLower());
                                if (!answerForQuestionString.Equals(""))
                                    answer.answerForQuestion = Convert.ToInt32(answerForQuestionString);


                                var requiresComment = $"{reader["ANSWER_REQUIRES_COMMENT"]}";
                                if (!String.IsNullOrEmpty(requiresComment))
                                    answer.requiresComment = Convert.ToBoolean(requiresComment.ToLower());

                                answer.questionReferences = new List<QuestionReference>();

                                if (scenario.questions.ContainsKey(questionLocation))
                                {
                                    scenario.questions[questionLocation].answerList.Add(answer.answerID, answer);
                                }
                                previousAnswerID = answerID;

                            }


                            // create a new question reference and add it to the current answer
                            QuestionReference questionReference = new QuestionReference();

                            var questionReferenceString = $"{reader["QUESTION_REFERENCE_ID"]}";
                            if (!questionReferenceString.Equals(""))
                            {
                                string questionReferenceID = $"{reader["QUESTION_REFERENCE_ID"]}";
                                string questionReferenceProbability = $"{reader["PROBABILITY"]}";
                                if (!questionReferenceID.Equals(""))
                                    questionReference.questionReference = Convert.ToInt32(questionReferenceID);
                                if (!questionReferenceProbability.Equals(""))
                                    questionReference.questionReferenceProbability =
                                        Convert.ToInt32(questionReferenceProbability);

                                scenario.questions[questionLocation].answerList[answerID].questionReferences
                                    .Add(questionReference);
                            }

                        }
                        previousQuestionID = questionLocation;
                    }
                    if (previousQuestionID != -1)
                    {
                        if (scenario.questions[previousQuestionID].isActive)
                        {
                            scenario.questionsActive.Add(previousQuestionID, scenario.questions[previousQuestionID]);
                        }

                    }
                    HttpContext.Current.Session["currentScenario"] = scenario;
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN09";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * On completion of a scenario, put an entry in the SCENARIOS_TO_GRADE table for teachers/admins to grade
         */

        public static int insertScenarioToGrade(string studentId, int scenarioId, int? grade, SqlConnection connection)
        {
            int newScenarioToGrade;
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "INSERT INTO SCENARIOS_TO_GRADE " +
                                  "(" +
                                  "STG_STUDENT_ID, " +
                                  "STG_SCENARIO_ID, " +
                                  "STG_GRADE" +
                                  ") " +
                                  "VALUES " +
                                  "(" +
                                  "@studentID, " +
                                  "@scenarioID, " +
                                  "@grade" +
                                  "); " +
                                  "" +
                                  "SELECT SCOPE_IDENTITY();"
                };

                var param = new SqlParameter("@studentID", SqlDbType.NVarChar, 128)
                {
                    Value = studentId
                };
                command.Parameters.Add(param);

                var param2 = new SqlParameter("@scenarioID", SqlDbType.Int)
                {
                    Value = scenarioId
                };
                command.Parameters.Add(param2);


                var param3 = new SqlParameter("@grade", SqlDbType.Int)
                {
                    Value = (object)grade ?? DBNull.Value
                };
                command.Parameters.Add(param3);


                connection.Close(); connection.Open();
                command.Prepare();
                newScenarioToGrade = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN10";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return newScenarioToGrade;
        }

        /*
         * This function will return a list of scenarios to grade. 
         */

        public static ScenariosToGradeList getScenariosToGrade(SqlConnection connection)
        {
            var scenariosToGradeList = new ScenariosToGradeList();
            scenariosToGradeList.scenariosToGrade = new List<ScenariosToGrade>();
            try
            {
                var questionCommand = new SqlCommand(null, connection);

                questionCommand.CommandText = "SELECT SCENARIOS_TO_GRADE.STG_ID, " +
                                              "SCENARIOS.SCENARIO_ID, " +
                                              "SCENARIOS.SCENARIO_NAME, " +
                                              "STUDENTS.STUDENT_ID, " +
                                              "STUDENTS.STUDENT_NAME " +
                                              "FROM SCENARIOS_TO_GRADE " +
                                              "JOIN STUDENTS ON SCENARIOS_TO_GRADE.STG_STUDENT_ID = STUDENTS.STUDENT_ID " +
                                              "JOIN SCENARIOS ON SCENARIOS_TO_GRADE.STG_SCENARIO_ID = SCENARIOS.SCENARIO_ID " +
                                              "WHERE SCENARIOS_TO_GRADE.STG_GRADE IS NULL;";


                connection.Close(); connection.Open();
                questionCommand.Prepare();
                using (var reader = questionCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var scenariosToGrade = new ScenariosToGrade();
                        scenariosToGrade.stgID = Convert.ToInt32($"{reader["STG_ID"]}");
                        scenariosToGrade.stgScenarioID = Convert.ToInt32($"{reader["SCENARIO_ID"]}");
                        scenariosToGrade.stgScenarioName = $"{reader["SCENARIO_NAME"]}";
                        scenariosToGrade.stgStudentID = $"{reader["STUDENT_ID"]}";
                        scenariosToGrade.stgStudentName = $"{reader["STUDENT_NAME"]}";

                        scenariosToGradeList.scenariosToGrade.Add(scenariosToGrade);
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN11";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return scenariosToGradeList;
        }

        /*
         * This function takes in the ID for a scenario that needs to be graded, the scenario ID, and the student ID. It returns 
         * a list of all the questions the student answered as well as the selected answer and any comments the student made.
         */

        public static ScenarioToGrade getScenarioToGrade(int stgID, int stgScenarioID, string stgStudentID, SqlConnection connection)
        {
            var scenarioToGrade = new ScenarioToGrade();
            try
            {
                var questionCommand = new SqlCommand(null, connection);

                questionCommand.CommandText = "SELECT SCENARIOS.SCENARIO_NAME, " +
                                              "QUESTIONS.QUESTION_TITLE, " +
                                              "QUESTIONS.QUESTION_DESCRIPTION, " +
                                              "ANSWERS.ANSWER_TEXT, " +
                                              "STUDENT_ANSWERS.SA_STUDENT_ANSWER_TEXT, " +
                                              "SCENARIOS_TO_GRADE.STG_GRADE, " +
                                              "SCENARIOS_TO_GRADE.STG_COMMENTS " +
                                              "FROM STUDENT_ANSWERS " +
                                              "JOIN SCENARIOS ON STUDENT_ANSWERS.SA_SCENARIO_ID = SCENARIOS.SCENARIO_ID " +
                                              "JOIN QUESTIONS ON STUDENT_ANSWERS.SA_QUESTION_ID = QUESTIONS.QUESTION_ID " +
                                              "JOIN ANSWERS ON STUDENT_ANSWERS.SA_ANSWER_ID = ANSWERS.ANSWER_ID " +
                                              "JOIN SCENARIOS_TO_GRADE ON STUDENT_ANSWERS.SA_SCENARIO_TO_GRADE = SCENARIOS_TO_GRADE.STG_ID " +
                                              "WHERE STUDENT_ANSWERS.SA_SCENARIO_ID = @ScenarioID AND STUDENT_ANSWERS.SA_STUDENT_ID = @StudentID " +
                                              "AND SCENARIOS_TO_GRADE.STG_ID= @stgID";

                var param1 = new SqlParameter("@ScenarioID", SqlDbType.Int);
                param1.Value = stgScenarioID;
                questionCommand.Parameters.Add(param1);
                var param2 = new SqlParameter("@StudentID", SqlDbType.NVarChar, 128);
                param2.Value = stgStudentID;
                questionCommand.Parameters.Add(param2);
                var param3 = new SqlParameter("@stgID", SqlDbType.Int);
                param3.Value = stgID;
                questionCommand.Parameters.Add(param3);

                scenarioToGrade.stgID = stgID;
                scenarioToGrade.stgScenarioID = stgScenarioID;
                scenarioToGrade.stgStudentID = stgStudentID;
                scenarioToGrade.stgAnswersToGrade = new List<StudentAnswer>();

                connection.Close(); connection.Open();
                questionCommand.Prepare();
                using (var reader = questionCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var answer = new StudentAnswer();

                        var scenarioName = $"{reader["SCENARIO_NAME"]}";
                        var questionTitle = $"{reader["QUESTION_TITLE"]}";
                        var questionDescription = $"{reader["QUESTION_DESCRIPTION"]}";
                        var answerText = $"{reader["ANSWER_TEXT"]}";
                        var answerComments = reader["SA_STUDENT_ANSWER_TEXT"] == null
                            ? ""
                            : $"{reader["SA_STUDENT_ANSWER_TEXT"]}";

                        var stringGrade = $"{reader["STG_GRADE"]}";
                        if (!String.IsNullOrEmpty(stringGrade))
                            scenarioToGrade.stgGrade = Convert.ToInt32(stringGrade);
                        else
                            scenarioToGrade.stgGrade = 0;
                        scenarioToGrade.stgComments = $"{reader["STG_COMMENTS"]}"; 
                        scenarioToGrade.stgScenarioName = scenarioName;
                        answer.questionTitle = questionTitle;
                        answer.questionDescription = questionDescription;
                        answer.answerText = answerText;
                        answer.answerComments = answerComments;


                        scenarioToGrade.stgAnswersToGrade.Add(answer);
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN12";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return scenarioToGrade;
        }

        /*
         * After a teacher/admin submits a grade for a student, set that entry in the table to whatever grade the
         * scenario received and insert any comments the teacher had.
         */
        public static void addGradeForScenario(int stgId, int grade, string gradeComments, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE SCENARIOS_TO_GRADE " +
                                  "SET STG_GRADE = @grade, STG_COMMENTS=@gradeComments" +
                                  " WHERE STG_ID = @stgID"
                };

                var param = new SqlParameter("@stgId", SqlDbType.Int)
                {
                    Value = stgId
                };
                command.Parameters.Add(param);

                var param2 = new SqlParameter("@grade", SqlDbType.Int)
                {
                    Value = grade
                };
                command.Parameters.Add(param2);

                var param3 = new SqlParameter("@gradeComments", SqlDbType.VarChar, 500)
                {
                    Value = gradeComments
                };
                command.Parameters.Add(param3);


                connection.Close(); connection.Open();
                command.Prepare();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN13";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Activates a scenario in the database
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="connection"></param>
        public static void makeScenarioActive(int scenarioId, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE SCENARIOS SET SCENARIO_IS_ACTIVE = 1 WHERE SCENARIO_ID = @scenarioID"
                };

                var param = new SqlParameter("@scenarioID", SqlDbType.Int)
                {
                    Value = scenarioId
                };
                command.Parameters.Add(param);

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SEN06";
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