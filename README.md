# tuk-tuk
Simple in-memory (for now) event bus

#Sample Usage

    internal class Program
    {
    	private static void Main(string[] args)
    	{
    	  // put an instance into an IoC or service locator for global access
    		var bus = new TukBus();
    		var session = bus.Subscribe<MultipleOfFifteenEvent>(HandleEvent);
    		
    		Console.WriteLine("Outputting multiples of 15 between 1 and 10000...");
    		Run(bus);
    
    		Console.ReadLine();
    
    		// you could put this in a using statement, but understand that the notification queue may
    		// not be finished yet which is perfectly fine in eventual consistency situations where you 
    		// just want to catch anything that may affect your transaction mid-flow
    		bus.Dispose();
    	}
    
    	private static void HandleEvent(IEvent ev)
    	{
    		var e = ev as MultipleOfFifteenEvent;
    		if (e == null) return;
    
    		Console.WriteLine($"{e.Winner} is a multiple of 15!");
    	}
    
    	private static void Run(IEventBus bus)
    	{
    		for (var i = 0; i < 10000; i++)
    		{
    			if(i % 15 == 0)
    				bus.Publish(new MultipleOfFifteenEvent(i));
    		}
    	}
    
    	public class MultipleOfFifteenEvent : IEvent
    	{
    		public int Winner { get; }
    
    		public MultipleOfFifteenEvent(int winner)
    		{
    			Winner = winner;
    		}
    	}
    }
