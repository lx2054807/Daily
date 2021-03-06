#include <iostream>
#include <algorithm>
using namespace std;
const int seed = 1024;

#pragma region 线性同余法

int last_rand_01 = -1;
/// <summary>
/// 线性同余法
/// 用当前伪随机数乘A加B后对C取余得到的余数加上D作为下一个伪随机数
/// 最后的随机数范围根据C,D确定
/// 不具备不可预测性
/// </summary>
/// <param name="seed"></param>
/// <returns></returns>
int rand_01()
{
	int R = last_rand_01;
	int A = 17;
	int B = 824;
	int C = 10;	
	int D = 5;
	if (R == -1) 
	{
		last_rand_01 = (A * R + B) % C + D;
		return last_rand_01;
	}
	else
	{
		last_rand_01 = (A * R + B) % C + D;
		return last_rand_01;
	}
}

#pragma endregion

#pragma region 单向散列函数法
/// 从这里就没有实现只有原理了
/// 单向散列函数也叫Hash函数,是把任意长度输入改变为固定长度输出并且难以通过输出转换得到输入的函数
/// 最有名的是MD5,其他还有SHA,MAC,CRC等等
/// 产生随机的原理为通过种子初始化计数器的值,通过单向散列函数生成伪随机数列,计数器加1
/// 具有单向性,不可预测性
#pragma endregion

#pragma region 加密法
/// 使用密码法可以生成强伪随机数,可以使用AES对称加密或RSA公钥加密
/// 通过种子初始化内部状态,用密钥加密内部计数器的值,将加密的值作为随机数输出同时把计数器的值加1
/// 密码的机密性支撑了随机数的不可预测性
#pragma endregion

#pragma region calculate_pi
/// <summary>
/// 一个均匀分布的浮点数 = 随机生成很多很多一定范围内的点,构成一个矩(正方)形
/// 以正方形边长为半径的圆的四分之一面积在矩形内
/// 计算这些随机分布的点, 距离原点距离小于等于1的点在圆上,其他在正方形上
/// 所有圆上的点/所有正方形上的点 = pi*r*r/4 / r*r = pi / 4
/// </summary>
/// <returns></returns>
float calculate_pi()
{
	int counts = 100000000;
	int hits = 0;
	for(int i = 0; i<counts; i++)
	{
		float x = rand()/double(RAND_MAX), y = rand() / double(RAND_MAX);
		float dis = pow((x * x + y * y), 0.5f);
		if (dis <= 1.0)
		{
			hits++;
		}
	}
	return 4.0f * ((float)hits / counts);
}

#pragma endregion

int main() 
{
	cout << rand_01()<<endl;
	cout << rand_01() << endl;
	cout << rand_01() << endl;
	cout << rand_01() << endl;
	cout << calculate_pi() << endl;
	return 0;
}