
AnjLab.Browser = {
	check: function () {
		var ua = navigator.userAgent;
		
		this.isOpera		= !!window.opera;
		this.isKonqueror	= (ua.indexOf('Konqueror') >= 0);
		this.isSafari		= (ua.indexOf('Safari') >= 0);
		this.isFirefox		= (ua.indexOf('Firefox') >= 0);
		this.isMozilla		= (!this.isKonqueror && !this.isSafari && !this.isFirefox) && (ua.indexOf('Gecko') >= 0);
		this.isIE			= (ua.toLowerCase().indexOf("msie") >= 0) && (this.isOpera == false);
		
		this.isGecko	= (ua.indexOf('Gecko') >= 0);
		
		this.Other	=	!this.isOpera && 
						!this.isKonqueror && 
						!this.isSafari &&
						!this.isFirefox &&
						!this.isMozilla &&
						!this.isIE;
		
		this.version = {version:0};
		
		if(this.isGecko) {
			if(ua.match(/rv:([\d\.]+)/gi))	this.version.revision = RegExp.$1;
			
			if(this.isFirefox) {
				if(ua.match(/Firefox\/([\d\.]+)/gi)) this.version.version = RegExp.$1
			}
			if(this.isSafari) {
				if(ua.match(/Safari\/([\d\.]+)/gi)) this.version.version = RegExp.$1
			}
			if(this.isKonqueror) {
				if(ua.match(/Konqueror\/([\d\.]+)/gi)) this.version.version = RegExp.$1
			}
			
			if(this.isMozilla)	this.version.version = this.version.revision;
		}
		
		if(this.isIE) {
			if(ua.match(/MSIE (\d+)\.(\d+)/i)) {
				this.version.version = RegExp.$1;
				this.version.subversion = RegExp.$2;
			}
		}
	}
};

AnjLab.Browser.check();