using System.Collections.Generic;

[System.Serializable]
public class DivisionProblem
{
    public int numerator;
    public int denominator;
    public List<int> options;
    public int correct_option_index;

    // Default constructor required for deserialization
    public DivisionProblem() { }

    public DivisionProblem(int numerator, int denominator, List<int> options, int correctIndex)
    {
        this.numerator = numerator;
        this.denominator = denominator;
        this.options = options;
        this.correct_option_index = correctIndex;
    }
}
