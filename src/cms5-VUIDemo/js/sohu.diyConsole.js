/**
 * 可视化编辑控制台
 * @author levinhuang
 * @param {Object} opts 选项,如clSec:"sec",cssWsp:"#main"表示分栏的css类为sec,工作区域的css选择器为#main
 * @dependency sohu.diyEditor.js;sohu.diyArea.js
 * TODO:将顶部菜单部分逻辑移到单独的js文件diyMenuBar.js
 */
sohu.diyConsole=function(opts){
	//属性
	opts=$.extend({},{
		cssWsp:"#vstp_main",clSec:"vstp_sec",clSec0:"vstp_sec0",clSecSub:"vstp_subsec",
		clSecRoot:"vstp_col",clArea:"area",cssArea:".area",dfTop:100,
		clAreaStatic:"vstp_static",
		limitSec:390,
		scrollWrapMainginTop:0
		},opts);
	var _this=this;
	this.$Workspace=$(opts.cssWsp);
	this.$Layout=$("#vstp_areaTools");
	

	this.Areas=null;
	
	var p={opts:opts};
	p._$pageTip=$("#vstp_pageTip");
	p._$elmTool=$("#vstp_elmTool");
	/* 对话框jq对象 */
	p._$wSec=$("#vstp_wCfgSec");
	p._$wCode=$("#vstp_wCode");
	/* /对话框jq对象 */
	p._opts=opts;
	/* =/顶部交互菜单= */
	
	p.getWorkspaceBoundary=function(){
		var lastArea=_this.$Workspace.find(opts.cssArea+":last");
		if (lastArea.size() == 0) {
			return {
				lbleft: -2000,
				ubleft: 2000,
				lbtop: -2000,
				ubtop: 2000
			};
		};
		
		var lbtop=_this.$Workspace.offset().top;
		var ubtop=lastArea.height()+lastArea.offset().top;
		var lbleft=lastArea.offset().left;
		var ubleft=lastArea.width()+lbleft;
		
		return {
			lbleft:lbleft,
			ubleft:ubleft,
			lbtop:lbtop,
			ubtop:ubtop
		};
	};
	//body标签的鼠标事件
	p.onBodyClick=function(evt){
		//用户是否点击#editMenu
		var $t=$(evt.target);
		if($t.parents(".jqmWindow").length>0||$t.parents(".cedt_jqm").length>0) return;
		
		var b=p.getWorkspaceBoundary();
		if(evt.pageX<b.lbleft||evt.pageX>b.ubleft||evt.pageY>b.ubtop){//||evt.pageY<b.lbtop
			//sohu.diyDialog.Hide(true);
			//sohu.diyConsole.Preview();
			//反激活横切
			if(sohu.diyConsole.CurArea)
				sohu.diyConsole.CurArea.Deactive();
			//反激活分栏
			if(sohu.diyConsole.CurSec)
				sohu.diyConsole.CurSec.Deactive();
				
			//移除拖拽助手和内容蒙层
			sohu.diyConsole.Dragger.handle.hide();				
		};
	};
	p.setDocumentDim=function(){
		return;
		var fullheight, height;
		fullheight = sohu.diyConsole.InnerHeight();        
		height = fullheight - p.opts.scrollWrapMainginTop;
		_this.$Workspace.css("minHeight",height);
	};
	p.onLoaded=function(){
		//文档高度适应处理
		p.setDocumentDim();
		//横切工具条位置
		_this.$Layout.css({"top":p.opts.scrollWrapMainginTop+30});
	};
	/**
	 * Save current document.selection to sohu.diyConsole.DocSelection
	 */
	p.saveSelection=function(){
		if(sohu.diyConsole.CurElm.InlineEditable){
			sohu.diySelection.snap(sohu.diyConsole.CurElm.i$frame[0].iDoc());
		}else{
			sohu.diySelection.snap(document,sohu.diyConsole.CurElm.$Layout[0]);
		};
	};
	p.Init=function(){
		//公有属性引用
		sohu.diyConsole.$WinSec=p._$wSec;
		sohu.diyConsole.$WinCode=p._$wCode;
		//sohu.diyConsole.$WinPageBG=p._$wPageBG;
		sohu.diyConsole.$SecEditorModel=$("#vstp_area_editor");		
		sohu.diyConsole.SecEditor=new sohu.diyEditor({bos:_this});
		sohu.diyConsole.CTEditor=new sohu.diyCTEditor({bos:_this});	
		sohu.diyConsole.$AreaHolder=$("#vstp_areaHolder");
		sohu.diyConsole.$FlashHolder=$("#vstp_flashHolder").mouseenter(function(){
			if(sohu.diyConsole.$FlashHolder.CT){
				sohu.diyConsole.$FlashHolder.CT.Active();
			};
		});
		sohu.diyConsole.$CTHelper=$("#vstp_ctHelper");
		sohu.diyConsole.IsPreview=false;
		sohu.diyConsole.$Workspace=_this.$Workspace;
		//body鼠标事件
		sohu.diyConsole.$Body=$("body").click(p.onBodyClick);
		//window resize事件
		sohu.diyConsole.$Window=$(window).resize(function(evt){
			p.setDocumentDim();
			if(sohu.diyConsole.CurSec)
				sohu.diyConsole.CurSec.Editor.Reposition();
		});
		new sohu.diyMenuBar({});
		//弹框组件
		sohu.diyDialog.Init({console:_this,cssDragCTM:'document',onInit:sohu.diyDialog.onInit});
		//碎片编辑组件
		sohu.diyChipEditor.Init({singleton:true});
		//on page loaded
		$(document).ready(p.onLoaded);
		//自定义事件
		$(_this).bind("evtPreview",function(e){});
	};
	this.__p=p;
	//Init
	p.Init();
	this.Fire();
};
/**
 * 将编辑功能注入现有文档
 */
sohu.diyConsole.prototype.Fire=function(){
	var _this=this;
	//加载现有的焦点图flash
	this.loadFlash();
	//已有横切
	this.Areas=this.AreaList().map(function(i,o){
		var a=new sohu.diyArea({
			isNew:false,
			console:_this,
			onRemove:null,
			obj:$(o)
		});
		return a;
	});
	//更新背景元素引用
	sohu.diyConsole.$BodyBGA=$("#vstp_main .bodyBGA");
	sohu.diyConsole.$BodyBGB=$("#vstp_main .bodyBGB");	
};
/**
 * 加载现有flash
 */
sohu.diyConsole.prototype.loadFlash=function(){
	$(".vstp_flashWrap").each(function(i,o){
		var $o=$(o);
		var d=$.evalJSON($o.next().html());
		if(d.dummy) return;
		var f=new sohuFlash(d.swf,d.id,d.w,d.h,d.interval);
		f.addParam("quality", "high");
		f.addParam("wmode", "opaque");
		for(var n in d.v){
			f.addVariable(n,d.v[n]);
		};
		f.write(d.pid);
		f.data=d;
		window['F_'+d.pid]=f;
	});
};
/**
 * 重定位
 */
sohu.diyConsole.prototype.RePosition=function(){
	sohu.diyDialog.wAreaTool.Reposition();
};
/**
 * 设定激活的横切对象
 * @param {Object} target
 */
sohu.diyConsole.prototype.ActiveArea=function(target){
	//将上一个横切反激活
	if(sohu.diyConsole.CurArea){
		if(target&&(target.ID==sohu.diyConsole.CurArea.ID)) 
			return this;
		
		sohu.diyConsole.CurArea.Deactive();
	};
	//激活当前的横切
	sohu.diyConsole.CurArea=target;
	this.CurArea=target;
	return this;
};
/**
 * 关闭内容选择对话框
 */
sohu.diyConsole.prototype.CloseCTDialog=function(){
	sohu.diyConsole.CurSec.Editor.CloseCTDialog();
};
/**
 * 获取所有横切jquery对象
 */
sohu.diyConsole.prototype.AreaList=function(){
	var _this=this;
	var items= this.$Workspace.find(this.__p.opts.cssArea);
	//剔除channelNav和indexNav等含有static类的横切
	items=$.grep(items,function(o,i){
		if($(o).hasClass(_this.__p.opts.clAreaStatic)) return false;
		return true;
	});
	
	return $(items);
};
/**
 * 弹出一个确认对话框
 * @param {Object} opts
 */
sohu.diyConsole.prototype.Confirm=function(opts){
	var _this=this;
	opts=$.extend({},{
		title:"确认操作?",
		ct:"",
		height:140,
		width:"",
		position:"center",
		resizable:false,
		modal:true,
		yes:null,
		no:null,
		close:null
	},opts);
	
	var dlOpt={
		title:opts.title,
		resizable:opts.resizable,
		height:opts.height,
		width:opts.width,
		modal:opts.modal,
		position:opts.position,
		buttons:{
			"取消":function(){
				if(opts.no){opts.no(this);};
				$(this).dialog("close");
			},
			"确认":function(){
				if(opts.yes){opts.yes(this);};
				$(this).dialog("close");
			}
		},
		close:function(evt,ui){
			_this.__p._$pageTip.removeClass("confirm");
			if(opts.close){
				opts.close(evt,ui);
			};
		}
	};
	this.__p._$pageTip.addClass("confirm").html(opts.ct).dialog(dlOpt);
};
/**
 * 移除.txtLoading
 */
sohu.diyConsole.toggleLoading=function(){
	$(".vstp_txtLoading").toggle();
};
/*静态方法、对象*/
sohu.diyConsole.Dragger={
	obj:null,
	handle:$("#vstp_ctHandle"),
	cssHandle:'.vstp_dragHandle'
};
sohu.diyConsole.CurArea=null;
sohu.diyConsole.CurSec=null;			/* 当前鼠标所在的分栏 */
sohu.diyConsole.EditingSec=null;		/* 当前内联编辑的分栏 */
sohu.diyConsole.CurCT=null;
sohu.diyConsole.EditingCT=null;
sohu.diyConsole.CurElm=null;			/* current editing element */
sohu.diyConsole.$SecEditorModel=null; 	/* 分栏编辑器dom模型 */
sohu.diyConsole.DocSelection='';
sohu.diyConsole.InnerHeight=function() {
    var x,y;
    if (self.innerHeight) // all except Explorer
    {
		return self.innerHeight;
    }
    else if (document.documentElement && document.documentElement.clientHeight)         
    {
		// Explorer 6 Strict Mode
        return document.documentElement.clientHeight;
    }
    else if (document.body) // other Explorers
    {
        return document.body.clientHeight;
    };

};
/**
 * 根据颜色值获取完整的上、右、下、左四个方向的颜色值
 * @param {Object} c
 */
sohu.diyConsole.GetBorderColor=function(c){
	c=$.trim(c);
	if(c=="") return null;
	//默认情况下用jquery的css方法获取的rgb颜色值rgb(9, 168, 139)中含有空格，需先将这些空格去掉
	var reg=/\b,\s\b/g;
	c=c.replace(reg,",");//将", "替换为","
	var cList=c.split(" "),retVal={};
	$.each(cList,function(i,o){
		if(o.indexOf("#")!=0&&o.indexOf("rgb")!=0)
			cList[i]="none";
			
	});
	switch(cList.length){
		case 0:
			retVal= null;
		break;
		case 1:
			retVal.top=retVal.right=retVal.bottom=retVal.left=cList[0];
		break;
		case 2:
			retVal.top=retVal.bottom=cList[0];
			retVal.left=retVal.right=cList[1];
		break;
		case 3:
			retVal.top=cList[0];
			retVal.left=retVal.right=cList[1];
			retVal.bottom=cList[2];	
		break;
		case 4:
			retVal.top=cList[0];
			retVal.right=cList[1];
			retVal.bottom=cList[2];
			retVal.left=cList[3];
		break;
		default:
			retVal=null;
		break;
	};//switch
	return retVal;
};
/**
 * 获取指定jq dom对象的第idx个css class
 * @param {Object} $dom
 * @param {Object} idx
 */
sohu.diyConsole.GetClassName=function($dom,idx){
	var cl=$.trim($dom.attr("class"));
	if(cl=="") return "";
	
	cl=cl.split(" ");
	cl=$.grep(cl,function(o,i){
		if(o=="") return false;
		return true;
	});
	idx=idx||0;
	if(idx<0) idx=0;
	if(idx>=cl.length) idx-=1;
	return cl[idx];
};
/**
 * 检测字符串是否符合ID规则：字母、数字、下划线
 * @param {Object} str
 */
sohu.diyConsole.IsValidID=function(str){
	if(!StringUtils.isAlphanumeric(str.replace("_",""))) return false;
	return true;
};
/**
 * 预览-退出编辑状态
 */
sohu.diyConsole.Preview=function(flag){
	if(flag&&bos&&bos.Areas&&bos.Areas.length>0){
		$(bos.Areas).each(function(i,o){
			o.UnbindEvts();
		});
		sohu.diyConsole.IsPreview=true;
		//隐藏横切工具条
		bos.$Layout.hide();
		//触发自定义事件evtPreview以便通知订阅者
		$(bos).trigger("evtPreview");
	};
	
	if(!sohu.diyConsole.CurArea) return;
	/*
	if(!sohu.diyConsole.CurArea.IsActive) return;
	if(sohu.diyConsole.CurArea.IsEditing) return;
	*/
	//if(sohu.diyConsole.Dragger.ing) return;
	//移除分栏编辑器
	if(sohu.diyConsole.CurSec)
		sohu.diyConsole.CurSec.Editor.Editing("off");

	//反激活横切
	sohu.diyConsole.CurArea.Deactive();
	//反激活分栏
	if(sohu.diyConsole.CurSec)
		sohu.diyConsole.CurSec.Deactive();
	
};
/**
 * 加载编辑状态
 */
sohu.diyConsole.UnPreview=function(){
	if(bos&&bos.Areas&&bos.Areas.length>0){
		$(bos.Areas).each(function(i,o){
			o.BindEvts();
		});
		//显示横切工具条
		bos.$Layout.show();	
		sohu.diyConsole.IsPreview=false;	
	};	
};
/**
 * 元素选择快照
 */
sohu.diyConsole.SnapSelection=function(){
	if(sohu.diyConsole.CurElm.InlineEditable){
		sohu.diySelection.snap(sohu.diyConsole.CurElm.i$frame[0].iDoc());
	}else{
		sohu.diySelection.snap(document,sohu.diyConsole.CurElm.$Layout[0]);
	};
};
/**
 * 解析背景图地址
 * @param {Object} img
 */
sohu.diyConsole.ParseBGImg=function(img){
	img=img=="none"?"":img;
	img=img.replace('url("',"").replace('")',"");
	return img;
};
/**
 * Stop the default navigation behavior of the A tag.
 * @param {Object} evt
 */
sohu.diyConsole.OnStopNav=function(evt){
	//alert("onStopNav");
	evt.preventDefault();
	return true;
};
/**
 * 让指定的拖拽对象订阅window.scroll事件,修正拖拽出窗体之外后无法操作的问题
 * @param {Object} $t 拖拽对象的jquery dom.
 */
sohu.diyConsole.FixDraggable=function($t){
	//订阅window的scroll事件
	$(window).scroll(function(){
		var of;
		if((of=$t.offset()).top<0)
			$t.css("top",0);
		
		if(of.top>(of.t1=$(document).height()))
			$t.css("top",$(this).height()-$t.height());
			
	});
};
/**
 * 添加内容的工厂方法。根据需求往当前分栏内部或者当前碎片下方添加内容
 * @param {Object} ct 内容对象
 */
sohu.diyConsole.AddCT=function(ct){
	var dlgCT=sohu.diyDialog.Get("addContent");
	var ctMode=dlgCT.$Layout.data("mode");
	if(ctMode==0){//mode的定义见sohu.diyDialog_.js
		//往当前分栏内部加内容
		sohu.diyConsole.CurSec.AddContent(ct);
	}else{
		//往当前碎片下发添加内容
		sohu.diyConsole.CurCT.AddContent(ct);
	};
};
