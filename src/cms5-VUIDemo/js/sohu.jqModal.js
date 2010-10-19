/*
 * jqModal - Minimalist Modaling with jQuery
 *   (http://dev.iceburg.net/jquery/jqModal/)
 *
 * Copyright (c) 2007,2008 Brice Burgess <bhb@iceburg.net>
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 * 
 * $Version: 03/01/2009 +r14
 */
/**
 * @jqModal's revision for Sohu.com
 * @Revision author levinhuang
 * @Version-Sohu:v1.2010.08.131
 * @Desc:Remove the auto focus feature,and make this feature configurable!
 */
(function($) {
	$.fn.jqm=function(o){
		var p={
			overlay: 50,
			overlayClass: 'jqmOverlay',
			closeClass: 'jqmClose',
			trigger: '.jqModal',
			ajax: F,
			ajaxText: '',
			target: F,
			modal: F,
			toTop: F,
			onShow: F,
			onHide: F,
			onLoad: F,
			autoFocus:!F
		};
		return this.each(function(){
			if(this._jqm)
				return H[this._jqm].c=$.extend({},H[this._jqm].c,o);
			
			s++;
			//Set jqm index
			this._jqm=s;
			//H->cache the modal object in a hash table
			H[s]={
				c:$.extend(p,$.jqm.params,o),
				a:F,
				w:$(this).addClass('jqmID'+s),
				s:s
			};
			if(p.trigger)
				$(this).jqmAddTrigger(p.trigger);
		});//return
	};//$.fn.jqm
	//Apply closing trigger
	$.fn.jqmAddClose=function(e){return hs(this,e,'jqmHide');};
	//Apply opening trigger
	$.fn.jqmAddTrigger=function(e){return hs(this,e,'jqmShow');};
	//show
	$.fn.jqmShow=function(t){
		return this.each(function(){
			t=t||window.event;
			$.jqm.open(this._jqm,t);
		});
	};
	//hide
	$.fn.jqmHide=function(t){
		return this.each(function(){
			t=t||window.event;
			$.jqm.close(this._jqm,t)
		});
	};
	//Static methods or properties used by the jqModal plugin
	$.jqm = {
		hash:{},
		open:function(s0,t){
			var h=H[s0],c=h.c,cc='.'+c.closeClass,
			z=(parseInt(h.w.css('z-index'))),z=(z>0)?z:3000,
			o=$('<div></div>').css({
				height:'100%',
				width:'100%',
				position:'fixed',
				left:0,
				top:0,
				'z-index':z-1,
				opacity:c.overlay/100
			});
			//already in open state
			if(h.a)return F;
			//not in open state,just open it
			h.t=t;
			h.a=true;
			h.w.css('z-index',z);
 			if(c.modal){
				if(!A[0])
					L('bind');
				A.push(s0);
			}else if(c.overlay > 0)
				h.w.jqmAddClose(o);
 			else 
				o=F;
				
 			h.o=(o)?o.addClass(c.overlayClass).prependTo('body'):F;
 			if(ie6){
				$('html,body').css({height:'100%',width:'100%'});
				if(o){
					o=o.css({position:'absolute'})[0];
					for(var y in {Top:1,Left:1})
						o.style.setExpression(y.toLowerCase(),"(_=(document.documentElement.scroll"+y+" || document.body.scroll"+y+"))+'px'");
				};
			};//ie6
 			if(c.ajax){
				var r=c.target||h.w,u=c.ajax,r=(typeof r == 'string')?$(r,h.w):$(r),u=(u.substr(0,1) == '@')?$(t).attr(u.substring(1)):u;
  				r.html(c.ajaxText).load(u,function(){
					if(c.onLoad)
						c.onLoad.call(this,h);
					if(cc)
						h.w.jqmAddClose($(cc,h.w));
					e(h);
				});
			}else if(cc)
				h.w.jqmAddClose($(cc,h.w));

 			if(c.toTop&&h.o)
				h.w.before('<span id="jqmP'+h.w[0]._jqm+'"></span>').insertAfter(h.o);
					
 			(c.onShow)?c.onShow(h):h.w.show();
			e(h);
			return F;
		},//open
		close:function(s1){
			var h=H[s1];
			//already in close state
			if(!h.a)return F;
			//not in close state
			h.a=F;
	 		if(A[0]){
				A.pop();
				if(!A[0])
					L('unbind');
			};
	 		if(h.c.toTop&&h.o)
				$('#jqmP'+h.w[0]._jqm).after(h.w).remove();
	 
	 		if(h.c.onHide)
				h.c.onHide(h);
			else{
				h.w.hide();
				if(h.o)
					h.o.remove();
			}; 
			return F;
		},//close
		params:{}
	};
	
	var s=0,
	H=$.jqm.hash,
	A=[],
	ie6=$.browser.msie&&($.browser.version == "6.0"),
	F=false,
	i=$('<iframe src="javascript:false;document.write(\'\');" class="jqm"></iframe>').css({opacity:0}),
	e=function(h){
		if(ie6){
			if(h.o)
				h.o.html('<p style="width:100%;height:100%"/>').prepend(i);
			else if(!$('iframe.jqm',h.w)[0])
				h.w.prepend(i); 			
		};//ie6
		f(h);
	},
	f=function(h){
		if(!h.c.autoFocus) return;
		try{$(':input:visible',h.w)[0].focus();}catch(_){}
	},
	L=function(t){
		$()[t]("keypress",m)[t]("keydown",m)[t]("mousedown",m);
	},
	m=function(e){
		var h=H[A[A.length-1]],
		r=(!$(e.target).parents('.jqmID'+h.s)[0]);
		if(r)
			f(h);
		return !r;
	},
	hs=function(w,t,c){
		return w.each(function(){
			var s2=this._jqm;
			$(t).each(function(){
				if(!this[c]){
					this[c]=[];
					$(this).click(function(){
						for(var i in {jqmShow:1,jqmHide:1}){
							if(!this[i]) continue;
							for(var i0=0;i0<this[i].length;i0++){
								if(H[this[i][i0]]&&H[this[i][i0]].w)
									H[this[i][i0]].w[i](this);
								return F;
									
							};//for1					
						};//for0
					});//click
				};//if				
				this[c].push(s2);
			});//each
		});//return
	};//hs
})(jQuery);