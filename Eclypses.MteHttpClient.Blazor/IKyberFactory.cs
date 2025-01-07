namespace Eclypses.MteHttpClient
{
    /// <summary>
    /// Contract for the factory method to create a Kyber Helper.
    /// </summary>
    public interface IKyberFactory
    {
        /// <summary>
        /// Creates an instance of the KyberHelper that
        /// wraps the java script methods that deal with Kyber.       
        /// <param name="flavor"></param>
        /// <returns>An instance of the KyberHelper.</returns>
        IKyberHelper Create();
    }
}