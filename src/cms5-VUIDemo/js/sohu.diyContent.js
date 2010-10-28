/**
 * 类-内容、碎片
 * @author levinhuang
 * @param {Object} opts 选项{$obj,sec}
 */
sohu.diyContent=function(opts){
	opts=$.extend({},{cl:"vstp_ct",clOn:"vstp_ctOn",scale:false,clElm:"vstp_elm",isNew:true},opts||{});
	var _this=this;
	this.Meta=opts.ct;
	this.IsNew=opts.isNew;
	this.$Layout=null;							/*在Validate方法中构建*/
	this.Type=opts.ct.type;						/* 内容插件类型 */
	this.Sec=opts.sec;							/* 分栏 */
	this.Editor=this.Sec.Editor;				/* 分栏编辑器 */
	this.CTEditor=sohu.diyConsole.CTEditor;		/* 内容编辑器 */
	this.MaxWidth=this.Sec.Width;				/* 最大宽度 */
	this.onDomed=null;							/* 被添加到dom树后的回调函数 */
	this.IsFlash=false;							/* 是否flash焦点图内容 */
	this.FlashObj=null;							/* flash对象 */
	this.IsEditing=false;						/* 是否处于编辑状态 */
	this.IsActive=false;						/* 是否处于选中状态 */
	
	//private property
	var p={opts:opts};
	this.__p=p;
	
	/* 验证内容的有效性 并且构建$Layout*/
	this.Validate();
	if(!this.Validation.valid) return;
	
	/* ID */
	if ((this.ID = this.$Layout.attr("id")) == "") {
		this.ID = "ct_" + this.Type + "_" + StringUtils.RdStr(8);
		this.$Layout.attr("id",this.ID);
	};
	
	//是否flash
	this.IsFlash=this.$Layout.flash;
	if(this.IsFlash){
		if(this.IsNew){
			this.onDomed=function(mode){
				this.FlashObj=window["F_"+this.FlashData.pid];
			};	
		}else{
			this.FlashObj=window["F_"+this.FlashData.pid];
		};
		
	};
	/* Persistence to the dom tree */
	if(this.IsNew)
		this.Editor.UpdateCT(this,1);
	/* Load elements */
	this.LoadElements();
	
	this.BindEvts();
};
/**
 * 获取内容的维度信息
 * @return {Object} {x,y,w,h}
 */
sohu.diyContent.prototype.Dim=function(){
	return {
		x:this.$Layout.offset().left,
		y:this.$Layout.offset().top,
		w:this.$Layout.width(),
		h:this.$Layout.height(),
		w1:this.$Layout.outerWidth(),
		h1:this.$Layout.outerHeight()
	};
};
/**
 * 利用html编辑器编辑内容
 */
sohu.diyContent.prototype.DoEdit=function(){
	this.Editor.DialogCT("update");
};
/**
 * 编辑焦点图内容
 */
sohu.diyContent.prototype.EditFlash=function(){
	sohu.diyDialog.Show('wFlash');
};
/**
 * Notice related objects that current element is being edited.
 * TODO:Use event model to implement this,let related objects register the inline editing event.
 * @param {Object} state
 */
sohu.diyContent.prototype.InlineEdit=function(state){
	if(state=="on"){
		this.Sec.Active();
		this.Sec.InlineEditing=true;
		this.IsEditing=true;
		sohu.diyConsole.EditingSec=this.Sec;
		sohu.diyConsole.EditingCT=this;
	}else{
		this.Sec.Deactive();
		this.Sec.InlineEditing=false;
		this.IsEditing=false;
		//this.$Layout.trigger("mouseleave");
		sohu.diyConsole.EditingSec=null;
		sohu.diyConsole.EditingCT=null;
	}
};
/**
 * 验证当前内容是否有效
 */
sohu.diyContent.prototype.Validate=function(){
	var _this=this;
	this.SetValidation(true);
	
	var commonValidate=function(ct,msg){
		if(_this.IsNew){
			ct.$Layout=$(ct.Meta.html0).filter("."+ct.Type);
		}else{
			ct.$Layout=ct.Meta.$dom;
		};
		
		if(ct.$Layout.length==0||(!ct.$Layout.is("."+ct.__p.opts.cl))){
			ct.SetValidation(false,msg);
		};
	};
	
	switch(this.Type){
		case "ohmygod":
			this.SetValidation(false,"Html内容不符合可视化专题模板规范");
		break;
		case "shtable":
			this.$Layout=$(this.Meta.html0).filter("table");
			if(this.$Layout.length==0){
				this.SetValidation(false,"Html内容无表格标签");
			}else{
				//将$Layout替换成符合diy内容模板规范的对象
				this.$Layout=$("<div/>").addClass(this.__p.opts.cl+" "+this.Type+" clear").append(this.$Layout);
			}
		break;
		case "vstp_flash":
			this.$Layout=this.IsNew?$(this.Meta.html0):this.Meta.$dom;
			this.$FlashData=this.$Layout.find(".vstp_flashData");
			this.FlashData=$.evalJSON(this.$FlashData.html());
		break;
		default:
			commonValidate(this,"Html内容不符合模板规范");
		break;
	};
	if(this.Validation.valid){
		$.extend(this.$Layout,this.Meta);
	};
	return this;
};
/**
 * 设置验证信息
 * @param {Boolean} isValid 是否有效
 * @param {String} 验证结果提示信息
 */
sohu.diyContent.prototype.SetValidation=function(isValid,msg){
	this.Validation={
		valid:isValid,
		msg:msg||null
	};
	return this;
};
sohu.diyContent.prototype.LoadElements=function(){
	var _this=this;
	var items=this.$Layout.find("."+this.__p.opts.clElm);
	items.each(function(i,o){
		new sohu.diyElement({
			$dom:$(o),
			ct:_this
		});
	});
	return this;
};
/**
 * 移除可视化编辑注册的事件
 */
sohu.diyContent.prototype.UnbindEvts=function(){
	//移除内容事件
	this.$Layout.unbind(".edit");
	//移除元素事件
	//this.$Layout.find("."+this.__p.opts.clElm).trigger("evtUnbindEvt");
	return this;
};
/**
 * 绑定可视化编辑的事件
 */
sohu.diyContent.prototype.BindEvts=function(){
	var p={},_this=this;
	p.mouseEnter=function(evt){
		//已经选中
		if(_this.IsActive) return false;
		//如果自己处于编辑内容状态，则不显示编辑器
		if(_this.IsEditing) return false;
		//如果别人处于编辑状态，则不显示编辑器
		if(sohu.diyConsole.CurCT&&sohu.diyConsole.CurCT.IsEditing) return false;
		//如果处于拖拽状态 ，则不显示编辑器
		if(sohu.diyConsole.Dragger.ing) return false;
		
		if(sohu.diyConsole.CurCT){
			sohu.diyConsole.CurCT.IsActive=false;
		};
		
		_this.IsActive=true;
		_this.Editor.CurCT=_this;
		sohu.diyConsole.CurCT=_this;
		//_this.ToggleDragger("on");
		_this.CTEditor.Show();
		//Flash焦点图
		if(_this.IsFlash){
			var d=_this.Dim();
			sohu.diyConsole.$FlashHolder.css({
				top:d.y-1,
				left:d.x-1,
				height:d.h+1,
				width:d.w+1,
				opacity:0.5,
				display:'block'
			}).unbind("click").bind("click",function(evt){
				_this.InlineEdit("on");
				sohu.diyConsole.CurCT=_this;
				_this.EditFlash();
			});
		};
		return false;//stop bubbling
	};
	p.mouseLeave=function(evt){
		if(_this.Editor.CurArea.IsEditing||_this.IsEditing||(_this.diyConsole.CurCT&&_this.diyConsole.CurCT.IsEditing)) return false;
		//_this.ToggleDragger("off");
		sohu.diyConsole.CurCT=null;
	};
	
	//内容的鼠标事件
	this.$Layout.bind("mouseenter.edit",p.mouseEnter).bind("mouseleave.edit",p.mouseLeave);
	//自定义事件
	this.$Layout.unbind("evtBindEvt").bind("evtBindEvt",function(e){
		_this.BindEvts();
		//_this.$Layout.find("."+_this.__p.opts.clElm).trigger("evtBindEvt");
		return false;//停止冒泡
	});
	this.$Layout.bind("evtUnbindEvt.edit",function(e){
		_this.UnbindEvts();
		return false;//停止冒泡
	});
	return this;
};
/**
 * 删除碎片内容
 */
sohu.diyContent.prototype.Cls=function(){
	this.Sec.RemoveCTByID(this.ID);
	this.$Layout.remove();
};
/*静态方法*/
/**
 * 从现有的dom元素构建一个diyContent 对象
 * @param {Object} opts 选项 {sec,ct}
 */
sohu.diyContent.New=function(opts){
	return new sohu.diyContent(opts);
};
