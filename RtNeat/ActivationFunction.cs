namespace RtNeat;

public static class ActivationFunction
{

    private static Dictionary<ActivationFunctionType, Func<double, double>> Functions = new Dictionary<ActivationFunctionType, Func<double, double>>
    {
        {ActivationFunctionType.Sigmoid, Sigmoid},
        {ActivationFunctionType.Tanh, Tanh},
        {ActivationFunctionType.ReLU, ReLU},
        {ActivationFunctionType.LeakyReLU, LeakyReLU},
        {ActivationFunctionType.ELU, ELU},
        {ActivationFunctionType.SELU, SELU},
        {ActivationFunctionType.SoftPlus, SoftPlus},
        {ActivationFunctionType.SoftSign, SoftSign},
        {ActivationFunctionType.SoftMax, SoftMax},
        {ActivationFunctionType.Sinusoid, Sinusoid},
        {ActivationFunctionType.Gaussian, Gaussian},
        {ActivationFunctionType.BentIdentity, BentIdentity},
    };
    
    
    public static double Call(ActivationFunctionType type, double x)
    {
        return Functions[type].Invoke(x);
    }

    private static double Sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }

    private static double Tanh(double x)
    {
        return Math.Tanh(x);
    }

    private static double ReLU(double x)
    {
        return Math.Max(0, x);
    }

    private static double LeakyReLU(double x)
    {
        return Math.Max(0.01 * x, x);
    }

    private static double ELU(double x)
    {
        return x > 0 ? x : Math.Exp(x) - 1;
    }

    private static double SELU(double x)
    {
        return x > 0 ? 1.0507009873554804934193349852946 * x : 1.0507009873554804934193349852946 * 1.6732632423543772848170429916717 * (Math.Exp(x) - 1);
    }
    
    private static double SoftPlus(double x)
    {
        return Math.Log(1 + Math.Exp(x));
    }
    
    private static double SoftSign(double x)
    {
        return x / (1 + Math.Abs(x));
    }
    
    private static double SoftMax(double x)
    {
        return Math.Exp(x) / (1 + Math.Exp(x));
    }
    
    private static double Sinusoid(double x)
    {
        return Math.Sin(x);
    }
    
    private static double Gaussian(double x)
    {
        return Math.Exp(-x * x);
    }
    
    private static double BentIdentity(double x)
    {
        return (Math.Sqrt(x * x + 1) - 1) / 2 + x;
    }

    public static ActivationFunctionType GetRandomActivationFunction(Random? random = null)
    {
        var values = Enum.GetValues(typeof(ActivationFunctionType));
        random ??= new Random();
        return (ActivationFunctionType)(values.GetValue(random.Next(values.Length)) ?? ActivationFunctionType.Sigmoid);
    }
}