using System.Collections.Generic;

[System.Serializable]
public class DivisionProblem
{
    public string question;
    public List<int> options;
    public int correct_option_index;

    // Default constructor required for deserialization
    public DivisionProblem() { }

    public DivisionProblem(string question, List<int> options, int correctIndex)
    {
        this.question = question;
        this.options = options;
        this.correct_option_index = correctIndex;
    }
}
