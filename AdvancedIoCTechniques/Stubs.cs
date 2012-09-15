
using System;

namespace AdvancedIoCTechniques
{

public interface IThird { }
public interface IThird0 : IThird { }
public class Third0 : Disposable, IThird0 { }
public interface IThird1 : IThird { }
public class Third1 : Disposable, IThird1 { }

public interface ISecond { }
public interface ISecond0 : ISecond { }
public class Second0 : ISecond0
{
	public IThird0 Third0{get;private set;}
	public IThird1 Third1{get;private set;}
    public Second0(
		IThird0 third0,
		IThird1 third1
		) {
		Third0 = third0;
		Third1 = third1;
		}
}
public interface ISecond1 : ISecond { }
public class Second1 : ISecond1
{
	public IThird0 Third0{get;private set;}
	public IThird1 Third1{get;private set;}
    public Second1(
		IThird0 third0,
		IThird1 third1
		) {
		Third0 = third0;
		Third1 = third1;
		}
}
public interface ISecond2 : ISecond { }
public class Second2 : ISecond2
{
	public IThird0 Third0{get;private set;}
	public IThird1 Third1{get;private set;}
    public Second2(
		IThird0 third0,
		IThird1 third1
		) {
		Third0 = third0;
		Third1 = third1;
		}
}

public interface IFirst { }
public interface IFirst0 : IFirst { }
public class First0 : Disposable, IFirst0
{
	public ISecond0 Second0{get;private set;}
	public ISecond1 Second1{get;private set;}
	public ISecond2 Second2{get;private set;}
    public First0(
		ISecond0 second0,
		ISecond1 second1,
		ISecond2 second2
		) {
		Second0 = second0;
		Second1 = second1;
		Second2 = second2;
		}
}
public interface IFirst1 : IFirst { }
public class First1 : Disposable, IFirst1
{
	public ISecond0 Second0{get;private set;}
	public ISecond1 Second1{get;private set;}
	public ISecond2 Second2{get;private set;}
    public First1(
		ISecond0 second0,
		ISecond1 second1,
		ISecond2 second2
		) {
		Second0 = second0;
		Second1 = second1;
		Second2 = second2;
		}
}
public interface IFirst2 : IFirst { }
public class First2 : Disposable, IFirst2
{
	public ISecond0 Second0{get;private set;}
	public ISecond1 Second1{get;private set;}
	public ISecond2 Second2{get;private set;}
    public First2(
		ISecond0 second0,
		ISecond1 second1,
		ISecond2 second2
		) {
		Second0 = second0;
		Second1 = second1;
		Second2 = second2;
		}
}
public interface IFirst3 : IFirst { }
public class First3 : Disposable, IFirst3
{
	public ISecond0 Second0{get;private set;}
	public ISecond1 Second1{get;private set;}
	public ISecond2 Second2{get;private set;}
    public First3(
		ISecond0 second0,
		ISecond1 second1,
		ISecond2 second2
		) {
		Second0 = second0;
		Second1 = second1;
		Second2 = second2;
		}
}
public interface IFirst4 : IFirst { }
public class First4 : Disposable, IFirst4
{
	public ISecond0 Second0{get;private set;}
	public ISecond1 Second1{get;private set;}
	public ISecond2 Second2{get;private set;}
    public First4(
		ISecond0 second0,
		ISecond1 second1,
		ISecond2 second2
		) {
		Second0 = second0;
		Second1 = second1;
		Second2 = second2;
		}
}
}