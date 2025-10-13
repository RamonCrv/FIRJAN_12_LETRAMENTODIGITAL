using UnityEngine;

[System.Serializable]
public class PlayerScore
{
    [Header("Habilidades do Letramento Digital")]
    public int letramentoDigital;     // Letramento Digital (skill1)
    public int pensamentoAnalitico;   // Pensamento Analítico (skill2)
    public int curiosidade;           // Curiosidade (skill3)
    
    public PlayerScore()
    {
        letramentoDigital = 0;
        pensamentoAnalitico = 0;
        curiosidade = 0;
    }
    
    public PlayerScore(int skill1, int skill2, int skill3)
    {
        letramentoDigital = skill1;
        pensamentoAnalitico = skill2;
        curiosidade = skill3;
    }
    
    public override string ToString()
    {
        return $"Letramento Digital: {letramentoDigital}, Pensamento Analítico: {pensamentoAnalitico}, Curiosidade: {curiosidade}";
    }
}