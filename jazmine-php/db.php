<?php

$wallet_port = 0;
$server_port = 0;
$pwd = '';

if (@is_array($_SESSION) && (array_key_exists('pwd',$_SESSION)))
{
	$pwd = $_SESSION['pwd'];
}

if (@is_array($_SESSION) && (array_key_exists('wp',$_SESSION)))
{
	$wallet_port = intval($_SESSION['wp']);
}

if (@is_array($_SESSION) && (array_key_exists('sp',$_SESSION)))
{
	$server_port = intval($_SESSION['sp']);
}

$layout = file_get_contents('layout.html');
function output($content,$layout)
{
	$layout = str_replace('<!--Content-->',$content,$layout);
	return ($layout);
}


function get_info($sp)
{
	$host = '127.0.0.1:'.$sp;
	$uri = '/getinfo';
	
	$req=array();
	$req['jsonrpc']='2.0';
	$d=json_encode($req);
	$ch = curl_init($host.$uri);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "GET");                                                                     
	//curl_setopt($ch, CURLOPT_POSTFIELDS, $d);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	//curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json','Content-Length: ' . strlen($d)));
	$res = curl_exec($ch);
	return ($res);
}

function get_address($wp)
{
	if ($wp<1) return;
	$host = '127.0.0.1:'.$wp;
	$uri = '/json_rpc';
	$req=array();
	$req['method']='getAddresses';
	$req['jsonrpc']='2.0';
	$d=json_encode($req);
	$ch = curl_init($host.$uri);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");                                                                     
	curl_setopt($ch, CURLOPT_POSTFIELDS, $d);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json','Content-Length: ' . strlen($d)));
	$res = curl_exec($ch);
	return ($res);
}

function get_balance($wp,$address)
{
	if ($wp<1) return;
	$host = '127.0.0.1:'.$wp;
	$uri = '/json_rpc';
	$req=array();
	$req['method']='getBalance';
	$req['jsonrpc']='2.0';
	$params=array();
    $params['address']=$address;
    $req['params'] = $params;
	$d=json_encode($req);
	$ch = curl_init($host.$uri);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");                                                                     
	curl_setopt($ch, CURLOPT_POSTFIELDS, $d);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json','Content-Length: ' . strlen($d)));
	$res = curl_exec($ch);
	return ($res);
}

function send_transfer($wp,$address_from,$address_to,$amount)
{
	if ($wp<1) return;
	$host = '127.0.0.1:'.$wp;
	$uri = '/json_rpc';
	$req=array();
	$req['method']='sendTransaction';
	$req['jsonrpc']='2.0';
	$params=array();
	$params['anonymity']=0;
	$params['fee']=1;
	$params['addresses']=array();
    $params['addresses'][]=$address_from;
    $params['transfers']=array();
    $a = array();
    $a['address'] = $address_to;
    $a['amount'] = intval($amount);
    //$a['payment_id']=$payment_id;
    $params['transfers'][]=$a;
    //$params['paymentId']=$payment_id;
    $req['params'] = $params;
  	$d=json_encode($req);
	$ch = curl_init($host.$uri);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");                                                                     
	curl_setopt($ch, CURLOPT_POSTFIELDS, $d);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json','Content-Length: ' . strlen($d)));
	$res = curl_exec($ch);
	return ($res);
}

function get_unconfirmed($wp)
{
	if ($wp<1) return;
	$host = '127.0.0.1:'.$wp;
	$uri = '/json_rpc';
	$req=array();
	$req['method']='getUnconfirmedTransactionHashes';
	$req['jsonrpc']='2.0';
	$d=json_encode($req);
	$ch = curl_init($host.$uri);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");                                                                     
	curl_setopt($ch, CURLOPT_POSTFIELDS, $d);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json','Content-Length: ' . strlen($d)));
	$res = curl_exec($ch);
	return ($res);
}

function create_address($wp)
{
	if ($wp<1) return;
	$host = '127.0.0.1:'.$wp;
	$uri = '/json_rpc';
	$req=array();
	$req['method']='createAddress';
	$req['jsonrpc']='2.0';
	$d=json_encode($req);
	$ch = curl_init($host.$uri);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");                                                                     
	curl_setopt($ch, CURLOPT_POSTFIELDS, $d);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json','Content-Length: ' . strlen($d)));
	$res = curl_exec($ch);
	return ($res);
}

function get_transactions($wp,$address,$height)
{
	if ($wp<1) return;
	$host = '127.0.0.1:'.$wp;
	$uri = '/json_rpc';
	$req=array();
	$req['method']='getTransactions';
	$req['jsonrpc']='2.0';
	$params=array();
	$params['addresses']=array();
	$params['addresses'][]=$address;
	$params['firstBlockIndex']=20000;
	$params['blockCount']=$height-20000;
//    $params['paymentId']='';
    $req['params'] = $params;
	$d=json_encode($req);
//	print_r($d);
	$ch = curl_init($host.$uri);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");                                                                     
	curl_setopt($ch, CURLOPT_POSTFIELDS, $d);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json','Content-Length: ' . strlen($d)));
	$res = curl_exec($ch);
	return ($res);
}

function short_address($addr)
{
	return (substr($addr,0,12).'/'.substr($addr,strlen($addr)-12));
}
