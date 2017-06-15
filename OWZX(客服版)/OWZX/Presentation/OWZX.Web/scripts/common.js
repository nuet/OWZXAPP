var uid = -1; //用户id
var isGuestSC = 0; //是否允许游客使用购物车(0代表不可以，1代表可以)
var scSubmitType = 0; //购物车的提交方式(0代表跳转到提示页面，1代表跳转到列表页面，2代表ajax提交)

$.ajaxSetup({
    cache: false //关闭AJAX缓存
});

$(function ()
{
    showScroll();
    function showScroll()
    {
        $(window).scroll(function ()
        {
            var scrollValue = $(window).scrollTop();
            scrollValue > 100 ? $('div[class=scroll]').fadeIn() : $('div[class=scroll]').fadeOut();
        });
        $('.top').click(function ()
        {
            $("html,body").animate({ scrollTop: 0 }, 200);
        });
    }
})
//判断是否是数字
function isNumber(val)
{
    var regex = /^[\d|\.]+$/;
    return regex.test(val);
}

//判断是否为整数
function isInt(val)
{
    var regex = /^\d+$/;
    return regex.test(val);
}

//判断是否为邮箱
function isEmail(val)
{
    var regex = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
    return regex.test(val);
}

//判断是否为手机号
function isMobile(val)
{
    var regex = /^[1][0-9][0-9]{9}$/;
    return regex.test(val);
}
//判断是否为有效的账户 用户名由英文、数字及"_"组成，5-20位字符 
function isRegisterUserName(s)
{
    var patrn = /^[a-zA-Z]{1}([a-zA-Z0-9]|[_]){4,20}$/;
    if (!patrn.exec(s)) return false
    return true
}
//判断是否为有效的密码 密码长度需在6到16位间，只含数字 字母 _
function isPwd(s)
{
    var patrn = /^(\w){6,16}$/;
    if (!patrn.exec(s)) return false
    return true
}

//搜索
function search()
{
    var word = document.getElementById('keyword').value;
    var tt = $(".header_select_sort span em").text();
    if (word == undefined || word == null || word.length < 1)
    {
        return false;
        //layer.msg("请输入关键词");
    }
    else
    {
        if (tt == "建材市场")
        {
            window.location.href = "/catalog/search?word=" + encodeURIComponent(word);
        }
        else if (key == "工程案例")
        {
            window.location.href = "/project/search?word=" + encodeURIComponent(word);
        }

    }
}

//获得购物车快照
function getCartSnap()
{
    if (isGuestSC == 0 && uid < 1)
    {
        return;
    }
    $("#cartSnap").show();
    $.get("/cart/snap", function (data)
    {
        getCartSnapResponse(data);
    })
}

//处理获得购物车快照的反馈信息
function getCartSnapResponse(data)
{
    try
    {
        var result = eval("(" + data + ")");
        alert(result.content);
    }
    catch (ex)
    {
        $("#cartSnap").html(data);
        $("#cartSnapProudctCount").html($("#csProudctCount").html());
    }
}

//关闭购物车快照
function closeCartSnap()
{
    $("#cartSnap").hide();
}

//处理添加商品到收藏夹的反馈信息
function addToFavoriteResponse(data)
{
    var result = eval("(" + data + ")");
    alert(result.content);
}

//添加商品到购物车
function addProductToCart(pid, buyCount)
{
    if (pid < 1)
    {
        layer.msg("请选择商品");
    }
    else if (isGuestSC == 0 && uid < 1)
    {
        layer.msg("请先登录");
    }
    else if (buyCount < 1)
    {
        layer.msg("请填写购买数量");
    }
    else if (scSubmitType != 2)
    {
        window.location.href = "/cart/addproduct?pid=" + pid + "&buyCount=" + buyCount;
    }
    else
    {
        $.get("/cart/addproduct?pid=" + pid + "&buyCount=" + buyCount, addProductToCartResponse)
    }
}

//处理添加商品到购物车的反馈信息
function addProductToCartResponse(data)
{
    var result = eval("(" + data + ")");
    layer.alert(result.content);
}

//购买商品
function buyProduct(pid, buyCount)
{
    if (pid < 1)
    {
        layer.msg("请选择商品");
    }
    else if (isGuestSC == 0 && uid < 1)
    {
        layer.msg("请先登录");
    }
    else if (buyCount < 1)
    {
        layer.msg("请填写购买数量");
    }
    else
    {
        $.get("/cart/buyproduct?pid=" + pid + "&buyCount=" + buyCount, buyProductResponse)
    }
}

//处理购买商品的反馈信息
function buyProductResponse(data)
{
    var result = eval("(" + data + ")");
    if (result.state == "success")
    {
        window.location.href = result.content;
    }
    else
    {
        layer.alert(result.content);
    }
}


//获得选中的购物车项键列表
function getSelectedCartItemKeyList()
{
    var valueList = new Array();
    $("#cartBody input[type=checkbox][name=cartItemCheckbox]:checked").each(function ()
    {
        valueList.push($(this).val());
    })

    if (valueList.length < 1)
    {
        //当取消全部商品时,添加一个字符防止商品全部选中
        return "_";
    }
    else
    {
        return valueList.join(',');
    }
}

//设置选择全部购物车项复选框
function setSelectAllCartItemCheckbox()
{
    var flag = true;
    $("#cartBody input[type=checkbox][name=cartItemCheckbox]:not(:checked)").each(function ()
    {
        flag = false;
        return false;
    })

    if (flag)
    {
        $("#selectAllBut_top").prop("checked", true);
        $("#selectAllBut_bottom").prop("checked", true);
    }
    else
    {
        $("#selectAllBut_top").prop("checked", false);
        $("#selectAllBut_bottom").prop("checked", false);
    }
}

//删除购物车中商品
function delCartProduct(pid, pos)
{
    if (isGuestSC == 0 && uid < 1)
    {
        layer.alert("请先登录");
        return;
    }
    if (pos == 0)
    {
        $.get("/cart/delproduct?pid=" + pid + "&pos=" + pos + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), function (data)
        {
            try
            {
                layer.alert(val("(" + data + ")").content);
            }
            catch (ex)
            {
                $("#cartBody").html(data);
                setSelectAllCartItemCheckbox();
            }
        })
    }
    else
    {
        $.get("/cart/delproduct?pid=" + pid + "&pos=" + pos, function (data)
        {
            try
            {
                layer.alert(val("(" + data + ")").content);
            }
            catch (ex)
            {
                $("#cartSnap").html(data);
                $("#cartSnapProudctCount").html($("#csProudctCount").html());
            }
        })
    }
}

//清空购物车
function clearCart(pos)
{
    if (isGuestSC == 0 && uid < 1)
    {
        layer.alert("请先登录");
        return;
    }
    $.get("/cart/clear?pos=" + pos, function (data)
    {
        try
        {
            alert(eval("(" + data + ")").content);
        }
        catch (ex)
        {
            if (pos == 0)
            {
                $("#cartBody").html(data);
            }
            else
            {
                $("#cartSnap").html(data);
                $("#cartSnapProudctCount").html("0");
            }
        }
    })
}

//改变商品数量
function changePruductCount(pid, buyCount)
{
    if (!isInt(buyCount))
    {
        layer.msg('请输入数字', { icon: 2 });
    }
    else if (isGuestSC == 0 && uid < 1)
    {
        layer.msg("请先登录", { icon: 2 });
    }
    else
    {
        var key = "0_" + pid;
        $("#cartBody input[type=checkbox][value=" + key + "]").each(function ()
        {
            $(this).prop("checked", true);
            return false;
        })
        $.get("/cart/changeproductcount?pid=" + pid + "&buyCount=" + buyCount + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), function (data)
        {
            try
            {
                layer.msg(eval("(" + data + ")").content, { icon: 2 });
            }
            catch (ex)
            {
                $("#cartBody").html(data);
                setSelectAllCartItemCheckbox();
            }
        })
    }
}

//取消或选中购物车项
function cancelOrSelectCartItem()
{
    if (isGuestSC == 0 && uid < 1)
    {
        layer.msg("请先登录");
        return;
    }
    $.get("/cart/cancelorselectcartitem?selectedCartItemKeyList=" + getSelectedCartItemKeyList(), function (data)
    {
        try
        {
            layer.alert(eval("(" + data + ")").content);
        }
        catch (ex)
        {
            $("#cartBody").html(data);
            setSelectAllCartItemCheckbox();
        }
    })
}

//取消或选中全部购物车项
function cancelOrSelectAllCartItem(obj)
{
    if (isGuestSC == 0 && uid < 1)
    {
        layer.msg("请先登录");
        return;
    }
    if (obj.checked)
    {
        $.get("/cart/selectallcartitem", function (data)
        {
            try
            {
                layer.alert(eval("(" + data + ")").content);
            }
            catch (ex)
            {
                $("#cartBody").html(data);
            }
        })
    }
    else
    {
        $("#cartBody input[type=checkbox]").each(function ()
        {
            $(this).prop("checked", false);
        })
        $("#totalCount").html("0");
        $("#productAmount").html("0.00");
        $("#fullCut").html("0");
        $("#orderAmount").html("0.00");
    }
}

//前往确认订单
function goConfirmOrder()
{
    if (isGuestSC == 0 && uid < 1)
    {
        layer.msg("请先登录");
        return;
    }
    var valueList = new Array();
    $("#cartBody input[type=checkbox][name=cartItemCheckbox]:checked").each(function ()
    {
        valueList.push($(this).val());
    })

    if (valueList.length < 1)
    {
        layer.msg("请先选择购物车商品");
    }
    else
    {
        $("#selectedCartItemKeyList").val(valueList.join(','));
        document.forms[0].submit();
    }
}

//ajax方法
function ajax(_url, _data, _callback, _isWaiting, _waitingMsg, _type, _datatype) {
    if (_isWaiting == undefined) { _isWaiting = true; }
    $.ajax({
        type: _type == undefined ? "post" : _type,
        url: _url,
        data: _data,
        async: false,
        dataType: _datatype == undefined ? "json" : _datatype,
        timeout: 30000,
        beforeSend: function (XMLHttpRequest) { if (_isWaiting) { } },
        success: _callback != undefined ? _callback : function (data) { },
        error: function (XMLHttpRequest, textStatus, errorThrown) { },
        complete: function (XMLHttpRequest, textStatus) { if (_isWaiting) { } }
    });
}

//格式化
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}


function strToJson(str) {
    if (!/^\{.*/.test(str)) { return null }
    if (str) {
        var json = eval('(' + str + ')');
        return json;
    }
    else {
        return null;
    }
}



//验证是否为手机号
function isMobile(mobile) {
    if (mobile == "") {
        return false;
    }
    if (isNaN(mobile) || (mobile.length != 11)) {
        return false;
    }
    var reg = /^((1[34578][0-9]{1}))\d{8}$/;
    if (!reg.test(mobile)) {
        return false;
    } else {
        return true;
    }
}

//检查邮箱
function EmailCheck(emai1) {
    if (emai1 == "") {
        return false;
    }
    var myreg = /^[0-9a-zA-Z_\-\.]+@[0-9a-zA-Z_\-]+(\.[0-9a-zA-Z_\-]+)*$/;
    if (!myreg.test(emai1))
    { return false; }
    else { return true; }
}
//验证是否数字
function IsNum(nubmer) {
    var re = /^[0-9]+.?[0-9]*$/;   //判断字符串是否为数字     //判断正整数 /^[1-9]+[0-9]*]*$/     
    if (!re.test(nubmer))
        return false;
    else
        return true;
}


/*弹出层*/
/*
	参数解释：
	title	标题
	url		请求的url
	id		需要操作的数据id
	w		弹出层宽度（缺省调默认值）
	h		弹出层高度（缺省调默认值）
*/
function layer_show(title, url, id, w, h) {
    if (title == null || title == '') {
        title = false;
    };
    if (url == null || url == '') {
        url = "404.html";
    };
    if (w == null || w == '') {
        w = 800;
    };
    if (h == null || h == '') {
        h = ($(window).height() - 50);
    };
    parent.layer.open({
        type: 2,
        title: title,
        fix: false, //不固定
        maxmin: true,
        shadeClose: true,
        shade: 0.8,
        area: [w + 'px', h + 'px'],
        content: url + "?id=" + id
    });
}

function getURLParameter(name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)', 'i').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || null;
}

//删除参数值
function delQueStr(url, ref) {
    var str = "";
    if (url.indexOf('?') != -1) {
        str = url.substr(url.indexOf('?') + 1);
    }
    else {
        return url;
    }
    var arr = "";
    var returnurl = "";
    var setparam = "";
    if (str.indexOf('&') != -1) {
        arr = str.split('&');
        for (i in arr) {
            if (arr[i].split('=')[0] != ref) {
                returnurl = returnurl + arr[i].split('=')[0] + "=" + arr[i].split('=')[1] + "&";
            }
        }
        return url.substr(0, url.indexOf('?')) + "?" + returnurl.substr(0, returnurl.length - 1);
    }
    else {
        arr = str.split('=');
        if (arr[0] == ref) {
            return url.substr(0, url.indexOf('?'));
        }
        else {
            return url;
        }
    }
}

function changeURLPar(url, ref, value) {
    var str = "";
    if (url.indexOf('?') != -1)
        str = url.substr(url.indexOf('?') + 1);
    else
        return url + "?" + ref + "=" + value;
    var returnurl = "";
    var setparam = "";
    var arr;
    var modify = "0";
    if (str.indexOf('&') != -1) {
        arr = str.split('&');
        for (i in arr) {
            if (arr[i].split('=')[0] == ref) {
                setparam = value;
                modify = "1";
            }
            else {
                setparam = arr[i].split('=')[1];
            }
            returnurl = returnurl + arr[i].split('=')[0] + "=" + setparam + "&";
        }
        returnurl = returnurl.substr(0, returnurl.length - 1);
        if (modify == "0")
            if (returnurl == str)
                returnurl = returnurl + "&" + ref + "=" + value;
    }
    else {
        if (str.indexOf('=') != -1) {
            arr = str.split('=');
            if (arr[0] == ref) {
                setparam = value;
                modify = "1";
            }
            else {
                setparam = arr[1];
            }
            returnurl = arr[0] + "=" + setparam;
            if (modify == "0")
                if (returnurl == str)
                    returnurl = returnurl + "&" + ref + "=" + value;
        }
        else
            returnurl = ref + "=" + value;
    }
    return url.substr(0, url.indexOf('?')) + "?" + returnurl;
}


Date.prototype.format = function (format) //author: meizz
{
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
    (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
      RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}
function formatTime(val) {
    var re = /-?\d+/;
    var m = re.exec(val);
    var d = new Date(parseInt(m[0]));
    // 按【2012-02-13 09:09:09】的格式返回日期
    return d.format("yyyy-MM-dd hh:mm:ss");
}
function formatDate(val) {
    var re = /-?\d+/;
    var m = re.exec(val);
    var d = new Date(parseInt(m[0]));
    // 按【2012-02-13 09:09:09】的格式返回日期
    return d.format("yyyy-MM-dd");
}

$("table thead th input:checkbox").on("click", function () {
    $(this).closest("table").find("tr > td:first-child input:checkbox").prop("checked", $("table thead th input:checkbox").prop("checked"));
});


/*检查是否有选中*/
function CheckPostBack() {
    if ($("input:checkbox[class='checkall']:checked").size() < 1) {
        return false;
    }
    return true;
}

