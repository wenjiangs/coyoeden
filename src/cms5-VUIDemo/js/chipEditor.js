/**
 * JS interactive logic for editing fragments
 * 碎片编辑器
 * @author levinhuang
 */
var chipEditor = function() {
    var p={},pub={};
	p.editors={};
	/* 事件处理 */	
	/**
	 * 获取碎片编号的逻辑
	 * @param {Object} $chip 碎片的jq dom对象
	 */
	p.getChipID=function($chip){
		var id=$chip.attr("cid");
		if((!id)||(id=="")){
			id="flag"+StringUtils.RdStr(8);
			$chip.attr("cid",id);
		};
		return id;	
	};
	/**
	 * 禁用a标签的默认导航行为
	 * @param {Object} evt
	 */
	p.stopNav=function(evt){
		evt.preventDefault();
		return true;		
	};
    //private area
    p.initVar = function(opts) {
		p._cssBold=opts.cssBold||'bb';
		p._cssColor=opts.cssColor||'cc';
		chipEditor.aTplStr=opts.aTplStr||'<a href="http://sohu.com" title="">sohu</a>';
		chipEditor.vdTplStr=opts.vdTplStr||'<a href="http://sohu.com" title=""><img src="http://images.sohu.com/uiue/vd.gif" alt="Video"/></a>';
		p._dlgModel=$(".chipEdt");
		p._$body=$("body");
		/* 用户的回调函数 */
		p.onUpPic=opts.onUpPic;
		p.onTest=opts.onTest;
		p.onSave=opts.onSave;
		p.onCancel=opts.onCancel;
		p.onGlobalRes=opts.onGlobalRes;
		p.onExternal=opts.onExternal;
		p.onFlashEdit=opts.onFlashEdit;
		p.onLoadHis=opts.onLoadHis;
		/* /用户的回调函数 */
	};
    p.onLoaded = function() { 

	};
    p.initEvents = function(opts) {
        $(document).ready(p.onLoaded);
		$(".acts a,button").button();
		$(".jqmCT a").bind('click.noNav',p.stopNav);
    };
    //public area
    pub.Init = function(opts) {
        p.initVar(opts);
        p.initEvents(opts);
    };
	/**
	 * 显示碎片编辑器
	 * @param {Object} chip 碎片的dom对象
	 * @param {Object} opts 其他选项
	 */
	pub.Show=function(chip,opts){
		opts=$.extend({tabs:[0,1,2]},opts||{});
		var dlg=null,$chip=$(chip),id=p.getChipID($chip);
		opts.isNew=false;
		if(!(dlg=p.editors[id])){
			opts.isNew=true;
			opts=$.extend({
				dlgModel:p._dlgModel,
				$body:p._$body,
				$chip:$chip,
				onUpPic:p.onUpPic,
				onTest:p.onTest,
				onSave:p.onSave,
				onCancel:p.onCancel,
				onGlobalRes:p.onGlobalRes,
				onExternal:p.onExternal,
				onFlashEdit:p.onFlashEdit,
				onLoadHis:p.onLoadHis								
			},opts);
			dlg=new chipEditor.Dialog(opts);
			p.editors[id]=dlg;
			
		};
		dlg.Show(opts);
	};
	pub.StopNav=p.stopNav;
	pub.$RowTxtA=$("#elmATpl .rowTxt");
	pub.$RowTitleA=$("#elmATpl .rowTitle");
	pub.$RowTxtA0=$("#elmATpl .rowTxt0");
	pub.$RowTitleA0=$("#elmATpl .rowTitle0");		
	pub.$RowLnkA=$("#elmATpl .rowLnk");
	pub.$RowImg=$("#elmImgTpl .rowImg");
	pub.$RowALT=$("#elmImgTpl .rowALT");
	pub.$RowLnkImg=$("#elmImgTpl .rowLnk");
    return pub;
} ();
/**
 * 碎片编辑器弹框类
 * @param {Object} opts
 */
chipEditor.Dialog=function(opts){
	opts=$.extend({},{
		tabIDTpl:'wVsp_tab',
		lblSort:'排序状态',
		lblSort1:'结束排序',
		lblTab0:'<strong class="alert">行修改状态</strong><em>请选择行</em>',
		clBold:'bb',
		clColor:'cc',
		onUpPic:null,
		onTest:null,
		onSave:null,
		onCancel:null,
		onGlobalRes:null,
		onExternal:null,
		onFlashEdit:null,
		onLoadHis:null
		},opts||{});
	var _this=this;
	this.Sorting=false;
	this._opts=opts;
	this.$Elm=null;//当前元素
	this.$Chip=opts.$chip;//当前碎片
	this.$Backup=this.$Chip.clone(true);
	this.SortElm=null;//当前排序元素
	
	//DOM引用
	this.$Layout=opts.dlgModel.clone().appendTo(opts.$body);
	this.$Code=this.$Layout.find('.txtVspC').val(this.$Chip.html());
	this.$Layout.find('.txtVspCS').textareaSearch({
		cssTextArea:_this.$Code,
		cssBtn:_this.$Layout.find('.btnVspCS')
	});
	this.$btnCode=this.$Layout.find(".external");//.globalRes,
	this.$ElmA=this.$Layout.find(".elmA");
	this.$ElmImg=this.$Layout.find(".elmImg");
	//事件处理
	this.$Chip.find("a").click(chipEditor.StopNav).click(function(evt){
		_this.Edit($(this));
	});
	//整体测试
	this.$Layout.find(".test").click(function(evt){
		if(opts.onTest){
			opts.onTest(_this);
		};
		return false;
	});
	//保存碎片
	this.$Layout.find(".save").click(function(evt){
		if(opts.onSave){
			opts.onSave(_this);
		};
		return false;
	});
	//取消
	this.$Layout.find(".cancel").click(function(evt){
		_this.$Chip.replaceWith(_this.$Backup.clone(true));
		_this.$Layout.jqmHide();
		if(opts.onCancel){
			opts.onCancel(_this);
		};
		return false;
	});
	//统一资源库
	this.$Layout.find(".globalRes").click(function(evt){
		if(opts.onGlobalRes){
			opts.onGlobalRes(_this);
		};
		return false;
	});
	//外包
	this.$Layout.find(".external").click(function(evt){
		if(opts.onExternal){
			opts.onExternal(_this);
		};
		return false;
	});
	//flash编辑
	this.$Layout.find(".btnFlashEdit").click(function(evt){
		if(opts.onFlashEdit){
			opts.onFlashEdit(_this);
		};
		return false;
	});
	//tab菜单
	var tabID="tab"+StringUtils.RdStr(8);
	this.$TabM=this.$Layout.find(".tabM").each(function(i,o){
		var $o=$(o);
		$o.attr("href",$o.attr("href").replace(opts.tabIDTpl,tabID));
	});
	//tab内容
	this.$TabC=this.$Layout.find(".tabC").each(function(i,o){
		var $o=$(o);
		$o.attr("id",$o.attr("id").replace(opts.tabIDTpl,tabID));
	});
	//排序按钮
	this.$Layout.find('.btnSort').click(function(evt){
		_this.ToggleSorting($(this));return false;
	});
	//tab
	this.$CT=this.$Layout.find(".jqmCT").tabs({
		select:function(evt,ui){
			//按钮的显隐
			if(ui.index==1){
				_this.$btnCode.removeClass("hide");
			}else{
				_this.$btnCode.addClass("hide");
			};
			//修改记录
			if(ui.index==2){
				if(opts.onLoadHis){
					opts.onLoadHis(_this);
				};
			};
		}
	});
	//上传浮层
	this.$UpPic=this.$CT.find(".uppic").draggable({handle:".upp_t",containment:'parent',axis:'y'});
	this.$UpPic.find(".up").click(function(evt){
		if(opts.onUpPic){
			opts.onUpPic(_this);
		};
	});
	this.$UpPic.find(".cls").click(function(evt){
		_this.$UpPic.slideUp("fast");
	});
	//jqm options
	this.jqmOpts={trigger:false,modal:false,overlay:false,autoFocus:false,beforeShow:null,afterShow:null,beforeHide:null,afterHide:null};
	this.jqmOpts.onShow=function(hash){
		var doShow=true;
		if(_this.jqmOpts.beforeShow){
			doShow=_this.jqmOpts.beforeShow(hash,_this);
		};
		if(doShow){
			hash.w.show();
			_this.jqmHash=hash;
			if(_this.jqmOpts.afterShow){
				_this.jqmOpts.afterShow(hash,_this);
			};
		};
	};
	this.jqmOpts.onHide=function(hash){
		var doHide=true;
		if(_this.jqmOpts.beforeHide){
			doHide=_this.jqmOpts.beforeHide(hash,_this);				
		};
		if(doHide){
			hash.w.hide();				
			if(_this.jqmOpts.modal){hash.o.remove();}; 
			//afterHide callback
			if(_this.jqmOpts.afterHide)
			{
				_this.jqmOpts.afterHide(hash,_this);
			};
		};	
	};
	
	//draggable
	this.$Layout.draggable({handle:".hd",containment:'body'});
	//margin-left
	this.$Layout.css("margin-left",-(this.$Layout.width()/2));
}; 
chipEditor.Dialog.prototype.Show=function(opts){
	var _this=this;
	opts=$.extend({tabs:[0,1,2]},opts||{});
	$.extend(this.jqmOpts,opts);
	this.IsNew=opts.isNew;
	if(!opts.isNew)
		this.Hide(opts);
	
	if((!opts.tabs)||opts.tabs.length==0){
		this.$CT.hide();
	}else{
		this.$TabM.hide();
		$.each(opts.tabs,function(i,o){
			_this.$TabM.eq(o).show();
		});
		this.$CT.tabs("select",opts.tabs[0]);
	};
		
	this.$Layout.jqm(this.jqmOpts).jqmShow();
};
chipEditor.Dialog.prototype.Hide=function(opts){
	$.extend(this.jqmOpts,opts||{});
	this.$Layout.jqm(this.jqmOpts).jqmHide();
	return this;
};
chipEditor.Dialog.prototype.ToggleSorting=function($i){
	var _this=this;
	if(this.Sorting){
		$i.removeClass("btnSort1").find("span").html(this._opts.lblSort);
		this.$CT.tabs("option","disabled",[]);
		this.$CT.tabs("option","selected",0);
		
		this.Sorting=false;
		//绑定click.edit事件
		this.$Chip.find("a").unbind("click.sort").bind("click.edit",this.Edit);
		//重置"可视化修改"和"代码修改"(由于元素发生了变化)
		this.$ElmA.empty().html(this._opts.lblTab0);
		this.$ElmImg.empty().html(this._opts.lblTab0);
	}else{
		$i.prev().trigger("click");
		$i.addClass("btnSort1").find("span").html(this._opts.lblSort1);
		this.$CT.tabs("option","disabled",[0,1,2]);
		this.Sorting=true;
		//移除碎片a标签的click.edit事件处理
		this.$Chip.find("a").unbind("click.edit").bind("click.sort",function(evt){
			_this.Sort($(this));
		});
	};
};
/**
 * 编辑某个元素
 * @param {Object} $elm
 */
chipEditor.Dialog.prototype.Edit=function($elm){
	var _t=0,_this=this;
	//改元素是否在编辑中
	if($elm.hasClass("ing")) return;
	
	$elm.addClass("ing");
	
	//检查元素类型
	if($elm.find("img").length>0){
		_t=1;//图片
	}else if($elm.parent().is("li")){
		_t=0;//列表
	}else{
		_t=2;//段落
	};
	
	var $elmList=$elm.parent().contents();
	
	if(this.$Elm) this.$Elm.removeClass("ing");
	this.$Elm=$elm;
	this.$Chip.addClass("on");
	
	var onShow=function(hash,dlg){	
		//第一个tab
		dlg.$ElmA.hide().empty();
		dlg.$ElmImg.hide().empty();
		switch(_t){
			case 0:
				$elmList.each(function(i,o){
					var $o=$(o);
					var tpl=null;
					if(o.nodeType!=1){
						//文本
						tpl=chipEditor.$RowTxtA.clone();
						dlg.$ElmA.append(tpl);
						var data={$obj:$o,$tpl:tpl};
						tpl.find("input").val($o.text()).bind("keyup",function(evt){
							var obj0=$(document.createTextNode(this.value));
							data.$obj.replaceWith(obj0);
							data.$obj=obj0;
						}).focus(_this.focusSelect);
						//删除按钮
						tpl.find(".btnDel").bind("click",function(evt){
							data.$obj.remove();
							tpl.remove();
						});
					}else if($o.is("a")){
						tpl=$([chipEditor.$RowTitleA.clone()[0],chipEditor.$RowLnkA.clone()[0]]);
						tpl.eq(0).find("input").val($o.html());//attr("title")
						tpl.eq(1).find("input").val($o.attr("href"));
						dlg.$ElmA.append(tpl);
						_this.BindIconEvts(tpl,{$obj:$o,$tpl:tpl,t:_t});
					};

				});
				_this.$ElmA.show();
			break;
			case 1:
				$elmList.each(function(i,o){
					var $o=$(o),tpl=null;
					if(o.nodeType!=1){
						//文本
						tpl=chipEditor.$RowTxtA.clone();
						_this.$ElmImg.append(tpl);
						var data={$obj:$o,$tpl:tpl,t:_t};
						tpl.find("input").val($o.text()).bind("keyup",function(evt){
							var obj0=$(document.createTextNode(this.value));
							data.$obj.replaceWith(obj0);
							data.$obj=obj0;
						}).focus(_this.focusSelect);
						//删除按钮
						tpl.find(".btnDel").bind("click",function(evt){
							data.$obj.remove();
							tpl.remove();
							return false;
						});								
					}else if($o.is("a")){
						var $img=$o.find("img");
						if($img.length>0){
							//图片链接
							tpl=$([chipEditor.$RowImg.clone()[0],chipEditor.$RowALT.clone()[0],chipEditor.$RowLnkImg.clone()[0]]);
							tpl.eq(0).find("input").val($img.attr("src"));
							tpl.eq(1).find("input").val($img.attr("alt"));
							tpl.eq(2).find("input").val($o.attr("href"));
							_this.$ElmImg.append(tpl);
							_this.BindIconEvts(tpl,{$obj:$o,$tpl:tpl,t:_t,$img:$img});								
						}else{
							//文字链接
							tpl=$([chipEditor.$RowTitleA.clone()[0],chipEditor.$RowLnkA.clone()[0]]);
							tpl.eq(0).find("input").val($o.html());
							tpl.eq(1).find("input").val($o.attr("href"));
							_this.$ElmImg.append(tpl);
							_this.BindIconEvts(tpl,{$obj:$o,$tpl:tpl,t:0});								
						};
					};
				});//each
				_this.$ElmImg.show();
			break;
			case 2:
				$elmList.each(function(i,o){
					var $o=$(o),tpl=null;
					if(o.nodeType!=1){
						//文本
						tpl=chipEditor.$RowTxtA0.clone();
						_this.$ElmA.append(tpl);
						var data={$obj:$o,$tpl:tpl,t:_t};
						tpl.find("input").val($o.text()).bind("keyup",function(evt){
							var obj0=$(document.createTextNode(this.value));
							data.$obj.replaceWith(obj0);
							data.$obj=obj0;
						}).focus(_this.focusSelect);						
					}else if($o.is("a")){
						tpl=$([chipEditor.$RowTitleA0.clone()[0],chipEditor.$RowLnkA.clone()[0]]);
						_this.$ElmA.append(tpl);
						//输入框事件处理
						tpl.eq(0).find("input").val($o.html()).keyup(function(evt){
							$o.html(this.value);
						}).focus(_this.focusSelect);
						tpl.eq(1).find("input").val($o.attr("href")).keyup(function(evt){
							$o.attr("href",this.value);
						}).focus(_this.focusSelect);
						
						
					};
				});			
				_this.$ElmA.show();
			break;
		};//switch
		//第二个tab
		dlg.$Code.val(dlg.$Chip.html());
		//上传按钮
		dlg.$Layout.find(".btnUpl").click(function(evt){
			dlg.$UpPic.show().effect("highlight");
			return false;
		});
					
	};//onShow
	var onHide=function(hash,dlg){
		if(dlg.$Elm){
			dlg.$Elm.removeClass("ing");
			dlg.$Elm=null;			
		};
		dlg.$Chip.removeClass("on");
	};//onHide
	this.Show({
		afterShow:onShow,
		afterHide:onHide,
	});
};
/**
 * 排序
 * @param {Object} $i
 */
chipEditor.Dialog.prototype.Sort=function($i){
	if($i.hasClass("sort")) return;
	if(!this.SortElm){
		this.SortElm=$i.addClass("sort");
		return;
	};
	//调换内容
	var temp=this.SortElm.html();
	this.SortElm.html($i.html());
	$i.html(temp);
	
	//结束本次操作
	this.SortElm.removeClass("sort");
	this.SortElm=null;
	if(this.$Elm)
		this.$Elm.removeClass('ing');
	//更新代码textarea
	this.$Code.val(this.$Chip.html());
};
/**
 * 图标事件处理
 * @param {Object} $tpl
 * @param {Object} data
 */
chipEditor.Dialog.prototype.BindIconEvts=function($tpl,data){
	var _this=this;
	//删除按钮
	$tpl.find(".btnDel").bind("click",function(evt){
		_this.DelElm(data);return false;
	});
	//复制按钮
	$tpl.find(".btnAdd").bind("click",function(evt){
		_this.AddElm(data);return false;
	});
	//加粗
	$tpl.find(".bold").bind("click",function(evt){
		_this.Bold(data);return false;
	});
	//颜色
	$tpl.find(".color").bind("click",function(evt){
		_this.Color(data);return false;
	});
	if(data.t==0){
		//标题输入框
		$tpl.eq(0).find("input").keyup(function(evt){
			data.$obj.html(this.value);
		}).focus(this.focusSelect);
		//链接输入框
		$tpl.eq(1).find("input").keyup(function(evt){
			data.$obj.attr("href",this.value);
		}).focus(this.focusSelect);
		//其他图标
		$tpl.eq(0).find('.icon').click(function(evt){
			$(this).next().toggle();
			return false;
		});
		//视频前
		$tpl.eq(0).find(".videoL").bind("click",function(evt){
			_this.InsertVDIcon({$obj:data.$obj,before:true});return false;
		});
		//视频后
		$tpl.eq(0).find(".videoR").bind("click",function(evt){
			_this.InsertVDIcon({$obj:data.$obj,before:false});return false;
		});			
	}else{
		//图片地址输入框
		$tpl.eq(0).find("input").change(function(evt){
			data.$img.attr("src",this.value);
		}).focus(this.focusSelect);
		//ALT输入框
		$tpl.eq(1).find("input").change(function(evt){
			data.$img.attr("alt",this.value);
		}).focus(this.focusSelect);
		//链接输入框
		$tpl.eq(2).find("input").change(function(evt){
			data.$obj.attr("href",this.value);
		}).focus(this.focusSelect);
	};

};
/**
 * 删除元素
 * @param {Object} data
 */
chipEditor.Dialog.prototype.DelElm=function(data){
	if(data.$obj){
		data.$obj.remove();
		data.$tpl.remove();
	};
};
/**
 * 加粗字体
 * @param {Object} data
 */
chipEditor.Dialog.prototype.Bold=function(data){
	if(!data.$obj) return false;
	if(data.$obj.hasClass(this._opts.clBold)){
		data.$obj.removeClass(this._opts.clBold).css("font-weight","normal");
		data.$tpl.eq(0).find("input").removeClass(this._opts.clBold);
	}else{
		data.$obj.addClass(this._opts.clBold).css("font-weight","bold");
		data.$tpl.eq(0).find("input").addClass(this._opts.clBold);
	};
};
/**
 * 着色
 * @param {Object} data
 */
chipEditor.Dialog.prototype.Color=function(data){
	if(!data.$obj) return false;
	if(data.$obj.hasClass(this._opts.clColor)){
		data.$obj.removeClass(this._opts.clColor).css("color",this._rawColor);
		data.$tpl.eq(0).find("input").removeClass(this._opts.clColor);
	}else{
		if(!this._rawColor)
			this._rawColor=data.$obj.css("color");
		
		data.$obj.addClass(this._opts.clColor).css("color","red");
		data.$tpl.eq(0).find("input").addClass(this._opts.clColor);			
	};
};
/**
 * 添加元素
 * @param {Object} data
 */
chipEditor.Dialog.prototype.AddElm=function(data){
	if(!data.$obj) return false;
	var txt0=$(document.createTextNode(" "));
	var a0=$(chipEditor.aTplStr);
	data.$obj.after(a0).after(txt0);
	a0.bind('click.noNav',chipEditor.StopNav);
	
	var tpl=chipEditor.$RowTxtA.clone();
	var data1={$obj:txt0,$tpl:tpl};
	//文字输入框事件(闭包的应用)
	tpl.find("input").val(" ").bind("keyup",function(evt){
		var obj0=$(document.createTextNode(this.value));
		data1.$obj.replaceWith(obj0);
		data1.$obj=obj0;
	}).focus(this.focusSelect);
	tpl.find(".btnDel").bind("click",function(evt){
		data1.$obj.remove();
		tpl.remove();
	});
	var tpl1=$([chipEditor.$RowTitleA.clone()[0],chipEditor.$RowLnkA.clone()[0]]);
	//输入框回填内容
	tpl1.eq(0).find("input").val(a0.html());
	tpl1.eq(1).find("input").val(a0.attr("href"));
	var data2={$obj:a0,$tpl:tpl1,t:0};
	//图标事件处理
	this.BindIconEvts(tpl1,data2);
	
	data.$tpl.eq(1).after(tpl1).after(tpl);
	return false;
};
chipEditor.Dialog.prototype.focusSelect=function(evt){
	$(this).select();
};
/**
 * 插入视频小图片
 * @param {Object} data
 */
chipEditor.Dialog.prototype.InsertVDIcon=function(data){
	var _this=this;
	var a=$(chipEditor.vdTplStr).bind("click.noNav",chipEditor.StopNav).bind('click.edit',function(evt){
		_this.Edit($(this));
	});
	if(data.before){
		data.$obj.before(a);
	}else{
		data.$obj.after(a);
	};
	return false;
};