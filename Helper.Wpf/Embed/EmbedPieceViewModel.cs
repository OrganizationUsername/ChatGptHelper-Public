using CommunityToolkit.Mvvm.ComponentModel;
using Helper.Core;

namespace Helper.Wpf.Embed;

public class EmbedPieceViewModel : ObservableObject
{
    private float[] _vector;
    private float _tempScore;

    public float TempScore
    {
        get => _tempScore;
        set => SetProperty(ref _tempScore, value);
    }

    public int EmbedThingId { get; set; }
    public string Text { get; set; }

    public float[] Vector
    {
        get => _vector;
        set => SetProperty(ref _vector, value);
    }

    public static EmbedPieceViewModel Get(EmbedPiece ep)
    {
        return new EmbedPieceViewModel
        {
            TempScore = 0,
            EmbedThingId = ep.EmbedThingId,
            Text = ep.Text,
            Vector = ep.Vector
        };
    }

    public static float DotProduct(float[] vector1, float[] vector2)
    {
        float dotProduct = 0;
        for (var i = 0; i < vector1.Length; i++) { dotProduct += vector1[i] * vector2[i]; }

        return dotProduct;
    }
}