 //编写程序实现重写Book类的equals()方法
 //有一个Book类
 class Book {
    //它有一个私有的id
    private String id;
    //通过构造器初始化id
    Book(String id){
        this.id=id;
    }
    //声明一个getId的方法，返回this.id
    public String getId(){
        return this.id;
    }
    
}
