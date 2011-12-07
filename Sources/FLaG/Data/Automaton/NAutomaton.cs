using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class NAutomaton
	{
		public void SaveFunctions (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveEndStatuses (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveAlphabet (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveStatuses (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveM (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveS (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveQ0 (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveDelta (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveSigma (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveQ (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public void SaveCortege (Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public bool IsLeft
		{
			get;
			set;
		}
		
		public int Number		
		{
			get;
			set;
		}
		
		public bool AddFunc(NTransitionFunc item)		
		{
			int index = Functions.BinarySearch(item);
			if (index < 0)
				Functions.Insert(~index,item);
			
			return index < 0;
		}	
		
		public bool AddEndStatus(NStatus item)		
		{
			int index = EndStatuses.BinarySearch(item);
			if (index < 0)
				EndStatuses.Insert(~index,item);
			
			return index < 0;
		}	
		
		public List<NStatus> EndStatuses
		{
			get;
			private set;
		}
		
		public List<NTransitionFunc> Functions
		{
			get;
			private set;
		}
		
		public NStatus InitialStatus
		{
			get;
			set;
		}
		
		public NAutomaton()
		{
			Functions = new List<NTransitionFunc>();	
			EndStatuses = new List<NStatus>();
		}
	}
}

