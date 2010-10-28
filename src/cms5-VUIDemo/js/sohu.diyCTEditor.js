/**
 * 内容编辑器
 * @author levinhuang
 */
sohu.diyCTEditor=function(opts){
	var _this=this;
	opts=$.extend({},{cssCTSelector:"#vstp_content_selector"},opts);
	//属性
	this.$Layout=$("#vstp_ctEditor");
	this.Console=opts.bos;
	this.CT=null;						/* 当前编辑器的目标内容 */
	
	var p={opts:opts};
	this.__p=p;
	
	this.$Toolbar=this.$Layout.find(".vstp_actions");	/* editor actions */
	this.$Overlay=this.$Layout.find(".vstp_overlay");	
	this.$menuOthers=this.$Layout.find(".vstp_others");
	//按钮事件注册
	this.$Toolbar.btn={
		addContent:this.$Toolbar.find(".vstp_a_content"),
		clear:this.$Toolbar.find(".vstp_a_clear"),
		editCode:this.$Toolbar.find(".vstp_a_code"),
		cfg:this.$Toolbar.find(".vstp_a_cfg"),
		cfgCT:this.$Toolbar.find(".vstp_a_cfgCT")
	};
	
	this.$Toolbar.btn.addContent.click(function(evt){_this.DialogCT();return false;});
	this.$Toolbar.btn.clear.click(function(evt){_this.Cls();return false;});
	this.$Toolbar.btn.editCode.click(function(evt){_this.DialogCode();return false;});
	this.$Toolbar.btn.cfgCT.click(function(evt){_this.DialogCTCfg();return false;});
	this.$Toolbar.btn.cfg.mouseenter(function(evt){_this.$menuOthers.show();return false;}).click(sohu.diyConsole.OnStopNav);
	
	//其他事件注册
	//this.$menuOthers.mouseleave(function(){_this.$menuOthers.hide();});
	
	//拖拽
	this.$Toolbar.find(".vstp_dragHandle").mousedown(function(){
		sohu.diyConsole.Dragger.obj=sohu.diyConsole.CurCT;
		sohu.diyConsole.Dragger.ing=true;
	}).mouseup(function(){
		sohu.diyConsole.Dragger.ing=false;
	});

	this.$CTWrap=$("#vstp_ctWrap");
	
	this.$Toolbar.isNew=true;
};
/**
 * 弹出代码对话框
 */
sohu.diyCTEditor.prototype.DialogCode=function(){
	if(sohu.diyConsole.CurArea.IsEditing) return;
	sohu.diyDialog.Show("code");
};
/**
 * 弹出设置对话框
 */
sohu.diyCTEditor.prototype.DialogCTCfg=function(){
	if(sohu.diyConsole.CurArea.IsEditing) return;
	sohu.diyDialog.Show("cfgCT");
};
/**
 * 弹出添加内容选择框
 * @param {String} mode mode="update"时编辑html内容
 */
sohu.diyCTEditor.prototype.DialogCT=function(mode){
	if(sohu.diyConsole.CurArea.IsEditing) return;
	sohu.diyDialog.Get("addContent").$Layout.data("mode",1);//0表示往分栏添加内容；1表示往碎片下方添加内容
	sohu.diyDialog.Show("addContent");
};
/**
 * 关闭内容选择框
 * @param {Object} opts 选项
 */
sohu.diyCTEditor.prototype.CloseCTDialog=function(opts){
	sohu.diyDialog.Hide();
};
/**
 * 更新diyEditor的内容-用于碎片
 * @param {diyContent} ct 内容对象
 * @param {int} mode 更新内容的类型 -1表示首追加
 */
sohu.diyCTEditor.prototype.UpdateCT=function(ct,mode){
	switch(mode){
		case -1:
			sohu.diyConsole.CurCT.$Layout.before(ct.$Layout);
		break;
		default:
			sohu.diyConsole.CurCT.$Layout.after(ct.$Layout);
		break;
	};
	if(ct.onDomed){
		ct.onDomed(mode);
	};
};
/**
 * 清空内容
 */
sohu.diyCTEditor.prototype.Cls=function(){
	if(sohu.diyConsole.CurArea.IsEditing) return;
	var _this=this;	
	sohu.diyDialog.doConfirm({
		text:"确认删除改碎片内容<br/>注意：删除后无法恢复！",
		onOK:function($jqm){
			_this.Detach();
			sohu.diyConsole.CurCT.Cls();
			$jqm.jqmHide();
		},
		beforeShow:function(hash){
			_this.Editing("on");
			return true;
		},
		afterShow:function(hash){
			//显示红色蒙层
			//_this.CurSec.Overlay("on");
			//_this.CT.Blink({speed:3000}).Blink(false);
			_this.Highlight();
		},
		afterHide:function(hash){
			_this.Editing("off");
			sohu.diyConsole.CurSec.Deactive();
			//_this.CurSec.Overlay("off");
			_this.Highlight('off');
		}
	});
	
};
/**
 * highlight the editor background
 * @param {Object} mode 'on' or 'off'
 */
sohu.diyCTEditor.prototype.Highlight=function(mode){
	mode=mode||'on';
	if(mode=='on'){
		this.$Overlay.addClass('vstp_overlay_hot');
	}else{
		this.$Overlay.removeClass("vstp_overlay_hot");
	};
	return this;
};
/**
 * 编辑器置入内容对象
 * @param {Object} ct
 */
sohu.diyCTEditor.prototype.AttachTo=function(ct){
	ct.$Layout.append(this.$Layout);
	this.CT=ct;
	return this;
};
sohu.diyCTEditor.prototype.Detach=function(){
	this.$Layout.detach();
	this.CT=null;
	return this;
};
/**
 * 重新定位碎片编辑器呈现
 */
sohu.diyCTEditor.prototype.Reposition=function(){		
	//获取当前的横切、分栏
	var d=sohu.diyConsole.CurCT.Dim();
	//this.$Toolbar.css({width:d.w1-11,top:d.y-25,left:d.x-1,opacity:0.9});/*宽要减去11个像素的留白;25是工具条高度*/
	this.$Toolbar.css({width:d.w1-11,top:-25,left:-1,opacity:0.9});
	//overlay
	//this.$Overlay.css({width:d.w1+1,top:d.y,left:d.x-1,opacity:0.9,height:d.h1+1});
	this.$Overlay.css({width:d.w1+1,top:0,left:-1,opacity:0.9,height:d.h1+1});
};
/**
 * 显示编辑器-即激活编辑器
 * @param {Object} opts
 */
sohu.diyCTEditor.prototype.Show=function(){
	if(this.$Toolbar.isNew){
		this.Reposition();
	}
	this.$Layout.show();
};
/**
 * 触发编辑事件。
 * @param {Object} mode "off"或"on"
 */
sohu.diyCTEditor.prototype.Editing=function(mode){
	if(mode=="on"){
		sohu.diyConsole.CurSec.IsAddingContent=true;
		sohu.diyConsole.CurArea.IsEditing=true;
		sohu.diyConsole.CurSec.$Layout.addClass("vstp_ing");
	}else{
		sohu.diyConsole.CurSec.IsAddingContent=false;
		sohu.diyConsole.CurArea.IsEditing=false;
		sohu.diyConsole.CurSec.$Layout.removeClass("vstp_ing");
	};
	return this;
};
/**
 * 隐藏
 */
sohu.diyCTEditor.prototype.Hide=function(){
	//return;
	if(!sohu.diyConsole.CurCT) return;
	if(sohu.diyConsole.CurSec.IsAddingContent) return;
	this.$Layout.hide();
};
