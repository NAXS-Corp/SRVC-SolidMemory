
/*
WebGLInject - Part of Simple Spectrum V2.1 by Sam Boyer.
*/

window.SimpleSpectrum = {};

window.AudioContext = (function(){
	var ACConsructor = window.AudioContext || window.webkitAudioContext; //keep a reference to the original function
	
	//console.log('AudioContext Constructor overriden');
		
	return function(){
		var ac = new ACConsructor();

		//console.log('AudioContext constructed');
		
		window.SimpleSpectrum.ac = ac;

		window.SimpleSpectrum.a = ac.createAnalyser();
		window.SimpleSpectrum.a.smoothingTimeConstant = 0;
			
		window.SimpleSpectrum.fa = new Uint8Array(window.SimpleSpectrum.a.frequencyBinCount); //frequency array, size of frequencyBinCount
			
		window.SimpleSpectrum.la = new Uint8Array(window.SimpleSpectrum.a.fftSize); //loudness array, size of fftSize
			
		window.SimpleSpectrum.a.connect(ac.destination); //connect the AnalyserNode to the AudioContext's destination.
			
		ac.actualDestination = ac.destination; //keep a reference to the destination.
			
		Object.defineProperty(ac, 'destination', { //replace ac.destination with our AnalyserNode.
			value: window.SimpleSpectrum.a,
			writable: false
		});		
			
		return ac; //send our modified AudioContext back to Unity.
	}
})();
