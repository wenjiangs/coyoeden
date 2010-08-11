/**
 * 可视化编辑元素
 * @dependency sohu.iframeEX,sohu.iEditable,jquery.cssEX,sohu.diyElementTool.js
 * @author levin
 */
sohu.diyElement=function(opts){
	opts=$.extend({},{cl:"elm",clOn:"elmOn",clCopyable:"elmc"},opts||{});
	var _this=this;
	this.CT=opts.ct;
	this.$Context=this.CT.$Layout;
	this.$Layout=opts.$dom;
	this.i$frame=null;/* Reference to the iframe editor */
	this.InlineEditable=true;
	this.tagName=this.$Layout[0].tagName.toLowerCase();
	this.Copyable=false;
	this.IsSelfCopyable=false;
	this.$CopyModel=null;
	
	/* private member variables */
	var p={opts:opts};
	this.__p=p;
	p.detectType=function(){
		if(_this.tagName=="img")
			_this.InlineEditable=false;
		
		//Copyable
		if(_this.$Layout.is(".elmc")){
			_this.Copyable=true;
			_this.IsSelfCopyable=true;
			_this.$CopyModel=_this.$Layout;
		}else{
			var o0=_this.$Layout.parents(".elmc");
			if(o0.length>0){
				o0=o0.eq(0);
				_this.Copyable=true;
				_this.IsSelfCopyable=false;
				_this.$CopyModel=o0;
			};
		};
	};
	p.onEditModeChange=function(dom){
		if (dom.iEditing) {
			_this.i$frame=_this.$Layout[0].i$frame;
			if(_this.$Layout.parent().is(".sec_hd")){
				sohu.diyDialog.Show("wSecHead");
			}else{
				sohu.diyDialog.Show("wText");
			};
			
			_this.CT.InlineEdit("on");
		}else{
			_this.i$frame=null;
			sohu.diyConsole.$CPKWrap.hide();
			sohu.diyConsole.$EditMenu.hide();
			_this.CT.InlineEdit("off");
		}
			
	};
	p.initEditable=function(){
		//inline editable element
		if(_this.InlineEditable){
			_this.$Layout.iEditable({
				onModeChange:p.onEditModeChange,
				i$frame:sohu.diyConsole.$ifEditor
			});
			return;
		};
		//img
		if(_this.tagName=="img"){
			_this.$Layout.click(function(evt){
				sohu.diyDialog.Show("wImage");
				_this.CT.InlineEdit("on");
				return false;
			});
			return;
		};
	};
	/* /private member variables */
	
	p.Init=function(){
		p.detectType();
		p.initEditable();
	};
	
	//初始化
	p.Init();
	
	//鼠标事件
	this.$Layout.mouseenter(function(evt){
		_this.$Layout.addClass(opts.clOn);
		/*
		if(_this.$Layout.hasClass(opts.clCopyable))
			sohu.diyElementTool.Trigger({type:"evtShow",elm:_this});
		*/
	});
	this.$Layout.mouseleave(function(evt){
		_this.$Layout.removeClass(opts.clOn);
		/*
		if(_this.$Layout.hasClass(opts.clCopyable))
			sohu.diyElementTool.Trigger({type:"evtHide",elm:_this});
		*/
	});
	this.$Layout.mousedown(function(evt){
		//sohu.diyConsole.$PopWins.hide();
		//sohu.diyConsole.$Menu.removeClass("on");
		if (sohu.diyConsole.CurElm) {
			sohu.diyConsole.CurElm.HideEditor(true);
			sohu.diyDialog.Hide(true);
			//sohu.diyDialog.Hide();
		};	
		sohu.diyConsole.CurElm=_this;
		
	});
	//屏蔽超链接
	this.$Context.find("a").css("cursor","text").click(function(evt){return false;});
};
/**
 * 关闭元素的编辑状态
 * @param {Object} ignoreCbk
 */
sohu.diyElement.prototype.HideEditor=function(ignoreCbk){
	if(this.InlineEditable){
		this.IFEdit("off",ignoreCbk);
	}else{
		sohu.diyConsole.$EditMenu.hide();
		this.CT.InlineEdit("off");
	}
	
	sohu.diyConsole.CurElm=null;
};
/**
 * 开启/关闭内联iframe编辑
 */
sohu.diyElement.prototype.IFEdit=function(mode,ignoreCbk){
	if(!this.InlineEditable) return;
	mode=mode||"on";
	mode=mode=="on"?mode:"off";
	this.$Layout[0].iEdit(mode,ignoreCbk||false);
};
